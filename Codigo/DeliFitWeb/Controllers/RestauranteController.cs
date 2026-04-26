using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Core.Identity.Data;
using System.Security.Cryptography;
using System.Linq;
using DeliFitWeb.Helpers;

namespace DeliFitWeb.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity>? _userManager;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly IEmailSender? _emailSender;

        public RestauranteController(IRestauranteService restauranteService,
                                     IMapper mapper,
                                     UserManager<UsuarioIdentity> userManager,
                                     RoleManager<IdentityRole> roleManager,
                                     IEmailSender emailSender)
        {
            _restauranteService = restauranteService;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        private static string GenerateSecurePassword(int length = 12)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string digits = "0123456789";
            if (length < 6) length = 6;

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            var chars = bytes.Select(b => valid[b % valid.Length]).ToArray();

            if (!chars.Any(c => digits.Contains(c)))
            {
                var pos = Math.Abs(BitConverter.ToInt32(bytes, 0)) % length;
                chars[pos] = digits[bytes[0] % digits.Length];
            }

            return new string(chars);
        }

        // GET: RestauranteController
        public ActionResult Index()
        {
            var listaRestaurantes = _restauranteService.GetRestaurantesAtivos();
            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);
            return View(listaRestaurantesModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ListarSolicitacoes()
        {
            var listaRestaurantes = _restauranteService.GetRestaurantesPendentes();
            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);
            return View(listaRestaurantesModel);
        }

        // GET: RestauranteController/Details/5
        public ActionResult Details(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);

            if (restaurante == null)
                return RedirectToAction(nameof(Index));

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            ViewBag.CanChangeStatus = false;
            return View(restauranteModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: RestauranteController/DetailsSolicitacao/5
        public ActionResult DetailsSolicitacao(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);

            if (restaurante == null)
                return RedirectToAction(nameof(ListarSolicitacoes));

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            ViewBag.CanChangeStatus = true;
            return View(restauranteModel);
        }

        // GET: RestauranteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestauranteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RestauranteViewModel restauranteModel)
        {
            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);

                if (restauranteModel.FotoFile != null && restauranteModel.FotoFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    restauranteModel.FotoFile.CopyTo(ms);
                    restaurante.Foto = ms.ToArray();
                }

                _restauranteService.Create(restaurante);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Serve a foto de um restaurante diretamente do banco para uso nas views via img src.
        /// </summary>
        [HttpGet]
        public ActionResult Foto(uint id)
        {
            var restaurante = _restauranteService.Get(id);

            if (restaurante?.Foto == null || restaurante.Foto.Length == 0)
                return NotFound();

            return File(restaurante.Foto, "image/jpeg");
        }

        // GET: RestauranteController/Edit/5
        public ActionResult Edit(uint? id)
        {
            uint restauranteId;

            // Se foi passado um ID, usa ele (Admin editando qualquer restaurante)
            if (id.HasValue)
            {
                restauranteId = id.Value;
            }
            else if (User.IsInRole("GerenteRestaurante"))
            {
                // Se nï¿½o foi passado ID, busca da sessï¿½o (gerente editando seu prï¿½prio restaurante)
                var restauranteIdSessao = GetRestauranteIdLogado();
                if (!restauranteIdSessao.HasValue)
                {
                    TempData["Error"] = "Não foi possível identificar o restaurante. Faça login novamente.";
                    return RedirectToAction("Home", "Restaurante");
                }
                restauranteId = restauranteIdSessao.Value;
            }
            else
            {
                return BadRequest("ID do restaurante não fornecido.");
            }

            Restaurante? restaurante = _restauranteService.Get(restauranteId);

            if (restaurante == null)
                return NotFound("Restaurante não encontrado.");

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            return View(restauranteModel);
        }

        // POST: RestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,GerenteRestaurante")]
        public ActionResult Edit(RestauranteViewModel restauranteModel)
        {
            // Se for gerente, verifica se estï¿½ editando o prï¿½prio restaurante
            if (User.IsInRole("GerenteRestaurante"))
            {
                var restauranteIdSessao = GetRestauranteIdLogado();
                if (!restauranteIdSessao.HasValue || restauranteIdSessao.Value != restauranteModel.Id)
                {
                    TempData["Error"] = "Você não tem permissão para editar este restaurante.";
                    return RedirectToAction("Home", "Restaurante");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var restaurante = _mapper.Map<Restaurante>(restauranteModel);

                    if (restauranteModel.FotoFile != null && restauranteModel.FotoFile.Length > 0)
                    {
                        // Nova foto enviada: converte e salva
                        using var ms = new MemoryStream();
                        restauranteModel.FotoFile.CopyTo(ms);
                        restaurante.Foto = ms.ToArray();
                    }
                    else
                    {
                        // Nenhuma foto nova: preserva a foto já existente no banco
                        var restauranteExistente = _restauranteService.Get(restauranteModel.Id);
                        restaurante.Foto = restauranteExistente?.Foto;
                    }

                    _restauranteService.Edit(restaurante);
                    TempData["Success"] = "Perfil atualizado com sucesso!";

                    if (User.IsInRole("GerenteRestaurante"))
                        return RedirectToAction("Home", "Restaurante");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao atualizar restaurante: {ex.Message}");
                }
            }

            return View(restauranteModel);
        }

        // GET: RestauranteController/Delete/5
        public ActionResult Delete(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);

            if (restaurante == null)
                return RedirectToAction(nameof(Index));

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            return View(restauranteModel);
        }

        // POST: RestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(uint id, RestauranteViewModel restauranteModel)
        {
            var restaurante = _restauranteService.Get(id);

            if (restaurante != null && !string.IsNullOrEmpty(restaurante.Email) && _userManager != null)
            {
                var user = await _userManager.FindByEmailAsync(restaurante.Email);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            _restauranteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: RestauranteController/SolicitarAdesao
        public ActionResult SolicitarAdesao()
        {
            return View();
        }

        // POST: RestauranteController/SolicitarAdesao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolicitarAdesao(RestauranteViewModel restauranteModel)
        {
            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);

                // Foto enviada na solicitação: converte e salva
                if (restauranteModel.FotoFile != null && restauranteModel.FotoFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    restauranteModel.FotoFile.CopyTo(ms);
                    restaurante.Foto = ms.ToArray();
                }

                // Restaurante começa como não validado (aguarda aprovação do admin)
                restaurante.Validado = false;

                _restauranteService.Create(restaurante);

                TempData["Success"] = "Solicitação enviada com sucesso! Aguarde a aprovação do administrador.";
                return RedirectToAction("Index", "Home");
            }

            return View(restauranteModel);
        }

        // POST: RestauranteController/AprovarSolicitacao/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AprovarSolicitacao(uint id)
        {
            try
            {
                var restaurante = _restauranteService.Get(id);
                if (restaurante == null)
                {
                    TempData["Error"] = "Restaurante não encontrado.";
                    return RedirectToAction(nameof(ListarSolicitacoes));
                }

                var email = restaurante.Email;

                if (!string.IsNullOrWhiteSpace(email) && _userManager != null && _roleManager != null && _emailSender != null)
                {
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        TempData["Error"] = $"Já existe um usuário cadastrado com o email {email}.";
                        return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                    }

                    var user = new UsuarioIdentity { UserName = email, Email = email };
                    var senha = GenerateSecurePassword(12);
                    var createResult = await _userManager.CreateAsync(user, senha);

                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "GerenteRestaurante");

                        restaurante.Validado = true;
                        _restauranteService.Edit(restaurante);

                        try
                        {
                            var assunto = "Solicitação aprovada - DeliFit";
                            var mensagem = $"Sua solicitação foi aprovada.\nUsuário: {email}\nSenha: {senha}\nAcesse: {Request.Scheme}://{Request.Host}/Identity/Account/Login";
                            await _emailSender.SendEmailAsync(email, assunto, mensagem);
                            TempData["Success"] = $"Solicitação aprovada! Email com credenciais enviado para {email}.";
                        }
                        catch (Exception)
                        {
                            TempData["Success"] = $"Solicitação aprovada! Porém não foi possível enviar o email. Senha gerada: {senha}";
                        }
                    }
                    else
                    {
                        var erros = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        TempData["Error"] = $"Erro ao criar usuário: {erros}";
                        return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                    }
                }
                else
                {
                    restaurante.Validado = true;
                    _restauranteService.Edit(restaurante);
                    TempData["Success"] = "Solicitação aprovada! (Email não configurado)";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao aprovar solicitação: {ex.Message}";
            }

            return RedirectToAction(nameof(ListarSolicitacoes));
        }

        // POST: RestauranteController/NegarSolicitacao/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NegarSolicitacao(uint id)
        {
            try
            {
                var restaurante = _restauranteService.Get(id);

                if (restaurante != null && !string.IsNullOrEmpty(restaurante.Email) && _userManager != null)
                {
                    var user = await _userManager.FindByEmailAsync(restaurante.Email);
                    if (user != null)
                        await _userManager.DeleteAsync(user);
                }

                _restauranteService.Delete(id);
                TempData["Success"] = "Solicitação negada e removida com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao negar solicitação: {ex.Message}";
            }

            return RedirectToAction(nameof(ListarSolicitacoes));
        }

        [Authorize(Roles = "GerenteRestaurante")]
        public ActionResult MeuRestaurante()
        {
            var restauranteId = GetRestauranteIdLogado();

            if (restauranteId.HasValue)
                return RedirectToAction(nameof(Details), new { id = restauranteId.Value });

            return NotFound("O restaurante associado a este utilizador não foi encontrado.");
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return Json(new { sucesso = false, mensagem = "CNPJ inválido." });

            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://brasilapi.com.br/api/cnpj/v1/{cnpj}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CnpjResponse>();

                return Json(new
                {
                    sucesso = true,
                    nomeRestaurante = string.IsNullOrEmpty(data?.nome_fantasia) ? data?.razao_social : data?.nome_fantasia,
                    cep = data?.cep,
                    rua = data?.logradouro,
                    numero = data?.numero,
                    bairro = data?.bairro,
                    cidade = data?.municipio,
                    estado = data?.uf
                });
            }

            return Json(new { sucesso = false, mensagem = "CNPJ não encontrado." });
        }

        [Authorize(Roles = "GerenteRestaurante")]
        public IActionResult Home()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult HomeAdmin()
        {
            return View();
        }

        private uint? GetRestauranteIdLogado()
        {
            var restauranteId = HttpContext.Session.GetRestauranteId();

            if (!restauranteId.HasValue)
            {
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var restaurante = _restauranteService.GetByEmail(userEmail);
                    if (restaurante != null)
                    {
                        HttpContext.Session.SetRestauranteId(restaurante.Id);
                        restauranteId = restaurante.Id;
                    }
                }
            }

            return restauranteId;
        }
    }
}
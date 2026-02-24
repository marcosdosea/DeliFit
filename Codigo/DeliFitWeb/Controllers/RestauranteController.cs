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
using DeliFitWeb.Areas.Identity.Data;
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
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            ViewBag.CanChangeStatus = false;

            return View(restauranteModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: RestauranteController/DetailsSolicitacao/5
        public ActionResult DetailsSolicitacao(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
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
                _restauranteService.Create(restaurante);
                return RedirectToAction(nameof(Index));
            }


            return RedirectToAction(nameof(Index));
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
                // Se não foi passado ID, busca da sessão (gerente editando seu próprio restaurante)
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
            {
                return NotFound("Restaurante não encontrado.");
            }

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            return View(restauranteModel);
        }

        // POST: RestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,GerenteRestaurante")]
        public ActionResult Edit(RestauranteViewModel restauranteModel)
        {
            // Se for gerente, verifica se está editando o próprio restaurante
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
                    _restauranteService.Edit(restaurante);
                    TempData["Success"] = "Perfil atualizado com sucesso!";

                    // Redireciona para página apropriada
                    if (User.IsInRole("GerenteRestaurante"))
                    {
                        return RedirectToAction("Home", "Restaurante");
                    }
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
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            return View(restauranteModel);
        }

        // POST: RestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, RestauranteViewModel restauranteModel)
        {
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
                _restauranteService.Create(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RestauranteController/AprovarSolicitacao/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AprovarSolicitacao(uint id)
        {
            var restaurante = _restauranteService.Get(id);
            if (restaurante != null)
            {
                var email = restaurante.Email;

                if (!string.IsNullOrWhiteSpace(email) && _userManager != null && _roleManager != null && _emailSender != null)
                {
                    var user = new UsuarioIdentity { UserName = email, Email = email };
                    var senha = GenerateSecurePassword(12);
                    var createResult = await _userManager.CreateAsync(user, senha);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "GerenteRestaurante");

                        restaurante.Validado = true;
                        _restauranteService.Edit(restaurante);

                        var assunto = "Solicitação aprovada - DeliFit";
                        var mensagem = $"Sua solicitação foi aprovada.\nUsuário: {email}\nSenha: {senha}\nAcesse: {Request.Scheme}://{Request.Host}/Identity/Account/Login";
                        await _emailSender.SendEmailAsync(email, assunto, mensagem);
                    }
                    else
                    {

                    }
                }
                else
                {
                    restaurante.Validado = true;
                    _restauranteService.Edit(restaurante);  
                }
            }
                return RedirectToAction(nameof(ListarSolicitacoes));
        }

        // POST: RestauranteController/NegarSolicitacao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NegarSolicitacao(uint id)
        {
            _restauranteService.Delete(id);

            return RedirectToAction(nameof(ListarSolicitacoes));
        }

        [Authorize(Roles = "GerenteRestaurante")]
        public ActionResult MeuRestaurante()
        {
            // Tenta buscar o ID do restaurante da sessão
            var restauranteId = GetRestauranteIdLogado();

            if (restauranteId.HasValue)
            {
                return RedirectToAction(nameof(Details), new { id = restauranteId.Value });
            }

            // Se não encontrou na sessão nem pelo email
            return NotFound("O restaurante associado a este utilizador não foi encontrado.");
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
            {
                return Json(new { sucesso = false, mensagem = "CNPJ inválido." });
            }

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

        // Método auxiliar para obter o ID do restaurante logado
        private uint? GetRestauranteIdLogado()
        {
            // Tenta buscar da sessão
            var restauranteId = HttpContext.Session.GetRestauranteId();

            if (!restauranteId.HasValue)
            {
                // Se não estiver na sessão, busca pelo email
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var restaurante = _restauranteService.GetByEmail(userEmail);

                    if (restaurante != null)
                    {
                        // Armazena na sessão para próximas requisições
                        HttpContext.Session.SetRestauranteId(restaurante.Id);
                        restauranteId = restaurante.Id;
                    }
                }
            }

            return restauranteId;
        }
    }
}

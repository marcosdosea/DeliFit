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
        private readonly IItemService _itemService;
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly ICarrinhoService _carrinhoService;
        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity>? _userManager;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly IEmailSender? _emailSender;

        public RestauranteController(IRestauranteService restauranteService,
                                     IItemService itemService,
                                     IPedidoService pedidoService,
                                     IClienteService clienteService,
                                     ICarrinhoService carrinhoService,
                                     IMapper mapper,
                                     UserManager<UsuarioIdentity> userManager,
                                     RoleManager<IdentityRole> roleManager,
                                     IEmailSender emailSender)
        {
            _restauranteService = restauranteService;
            _itemService = itemService;
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _carrinhoService = carrinhoService;
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

        // GET: RestauranteController/Details/5 (Admin)
        public ActionResult Details(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);

            if (restaurante == null)
                return RedirectToAction(nameof(Index));

            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            ViewBag.CanChangeStatus = false;
            return View(restauranteModel);
        }

        // GET: RestauranteController/VerEstabelecimento/5 (Cliente)
        [Authorize(Roles = "Cliente")]
        public ActionResult VerEstabelecimento(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);

            if (restaurante == null)
                return RedirectToAction("HomeCliente", "Cliente");

            var restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            var itens = _itemService.GetByRestaurante(id);
            var itensModel = _mapper.Map<List<ItemViewModel>>(itens);

            ViewBag.Itens = itensModel;
            return View(restauranteModel);
        }

        [Authorize(Roles = "Admin")]
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

            if (id.HasValue)
            {
                restauranteId = id.Value;
            }
            else if (User.IsInRole("GerenteRestaurante"))
            {
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
                        using var ms = new MemoryStream();
                        restauranteModel.FotoFile.CopyTo(ms);
                        restaurante.Foto = ms.ToArray();
                    }
                    else
                    {
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

                if (restauranteModel.FotoFile != null && restauranteModel.FotoFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    restauranteModel.FotoFile.CopyTo(ms);
                    restaurante.Foto = ms.ToArray();
                }

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

                if (string.IsNullOrWhiteSpace(email))
                {
                    TempData["Error"] = "O restaurante não possui email cadastrado.";
                    return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                }

                if (_userManager == null || _roleManager == null || _emailSender == null)
                {
                    TempData["Error"] = "Serviços de autenticação/email não configurados.";
                    return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                }

                var existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser != null)
                {
                    TempData["Error"] =
                        $"Já existe um usuário cadastrado com o email {email}.";

                    return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                }

                var user = new UsuarioIdentity
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var senha = GenerateSecurePassword(12);

                var createResult = await _userManager.CreateAsync(user, senha);

                if (!createResult.Succeeded)
                {
                    var erros = string.Join(", ",
                        createResult.Errors.Select(e => e.Description));

                    TempData["Error"] = $"Erro ao criar usuário: {erros}";

                    return RedirectToAction(nameof(DetailsSolicitacao), new { id });
                }

                await _userManager.AddToRoleAsync(user, "GerenteRestaurante");

                var mensagem = $@"
                    <h2>Sua solicitação foi aprovada!</h2>

                    <p>Seu restaurante agora faz parte da plataforma DeliFit.</p>

                    <hr>

                    <p>
                        <strong>Usuário:</strong> {email}
                    </p>

                    <p>
                         <strong>Senha:</strong> {senha}
                    </p>

                    <p>
                        Clique no link abaixo para acessar o sistema:
                    </p>
        
                    <p>
                        <a href='{Request.Scheme}://{Request.Host}/Identity/Account/Login'>
                        Acessar DeliFit
                    </a>
                    </p>";

                try
                {
                    await _emailSender.SendEmailAsync(
                        email,
                        "Solicitação aprovada - DeliFit",
                        mensagem
                    );

                    restaurante.Validado = true;

                    _restauranteService.Edit(restaurante);

                    TempData["Success"] =
                        $"Solicitação aprovada! Email enviado para {email}.";
                }
                catch (Exception ex)
                {
                    await _userManager.DeleteAsync(user);

                    TempData["Error"] =
                        $"Erro ao enviar email para o restaurante: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"Erro ao aprovar solicitação: {ex.Message}";
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

        /// <summary>
        /// Retorna os pedidos ativos do restaurante (status P, E, S) para o kanban.
        /// Pedidos finalizados (F) são excluídos — continuam no banco para consulta do cliente.
        /// Mapeamento: P=0 (RECEBIDO), E=1 (EM PREPARO), S=2 (EM ENTREGA)
        /// </summary>
        [Authorize(Roles = "GerenteRestaurante")]
        [HttpGet]
        public IActionResult GetPedidosRestaurante()
        {
            try
            {
                var restauranteId = GetRestauranteIdLogado();
                if (!restauranteId.HasValue)
                    return Json(new { erro = "Restaurante não identificado" });

                var statusCharToInt = new Dictionary<char, int>
                {
                    { 'P', 0 },
                    { 'E', 1 },
                    { 'S', 2 }
                    // 'F' é intencionalmente omitido: pedidos finalizados não aparecem no kanban
                };

                var pedidos = _pedidoService.GetAll()
                    .Where(p => p.IdRestaurante == restauranteId.Value
                                && p.Status.HasValue
                                && statusCharToInt.ContainsKey(p.Status.Value))
                    .OrderByDescending(p => p.Data)
                    .ToList();

                var resultado = pedidos.Select(p =>
                {
                    var carrinho = _carrinhoService.Get(p.IdCarrinho);
                    var cliente = carrinho != null ? _clienteService.Get(carrinho.IdCliente) : null;
                    var endereco = cliente?.Enderecos?.FirstOrDefault();

                    return new
                    {
                        id = p.Id,
                        data = p.Data,
                        preco = p.Preco,
                        status = statusCharToInt[p.Status!.Value],
                        nomeCliente = cliente?.Nome ?? "Cliente",
                        enderecoCliente = endereco != null
                            ? $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}"
                            : "Endereço não informado"
                    };
                }).ToList();

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o status de um pedido pelo restaurante.
        /// Mapeamento: 0=P (Recebido), 1=E (Em Preparo), 2=S (Em Entrega), 3=F (Finalizado)
        /// Ao finalizar (3→F) o pedido some do kanban mas permanece no banco para o cliente.
        /// </summary>
        [Authorize(Roles = "GerenteRestaurante")]
        [HttpPost]
        public IActionResult AtualizarStatusPedido([FromBody] AtualizarStatusPedidoRequest request)
        {
            try
            {
                if (request == null || request.PedidoId == 0)
                    return BadRequest("Dados inválidos");

                var pedido = _pedidoService.Get(request.PedidoId);
                if (pedido == null)
                    return NotFound("Pedido não encontrado");

                var restauranteId = GetRestauranteIdLogado();
                if (pedido.IdRestaurante != restauranteId)
                    return Forbid();

                // 0=P, 1=E, 2=S, 3=F
                char[] statusMap = { 'P', 'E', 'S', 'F' };
                if (request.NovoStatus < 0 || request.NovoStatus >= statusMap.Length)
                    return BadRequest("Status inválido");

                pedido.Status = statusMap[request.NovoStatus];
                _pedidoService.Edit(pedido);

                return Ok(new { sucesso = true, mensagem = "Pedido atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [Authorize(Roles = "GerenteRestaurante")]
        [HttpPost]
        public IActionResult AlternarStatusLoja()
        {
            try
            {
                var restauranteId = GetRestauranteIdLogado();
                if (!restauranteId.HasValue)
                    return BadRequest("Restaurante não identificado");

                var restaurante = _restauranteService.Get(restauranteId.Value);
                if (restaurante == null)
                    return NotFound("Restaurante não encontrado");

                restaurante.Validado = !restaurante.Validado;
                _restauranteService.Edit(restaurante);

                return Ok(new { sucesso = true, aberto = restaurante.Validado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
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
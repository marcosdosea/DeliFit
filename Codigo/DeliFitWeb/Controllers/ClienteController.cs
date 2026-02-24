using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Models;
using Core;
using Microsoft.AspNetCore.Identity;
using Core.Identity.Data;
using DeliFitWeb.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DeliFitWeb.Controllers
{
    public class ClienteController : Controller
    {

        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IRestauranteService _restauranteService;

        public ClienteController(IClienteService clienteService, IMapper mapper, UserManager<UsuarioIdentity> userManager, IRestauranteService restauranteService)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _userManager = userManager;
            _restauranteService = restauranteService;
        }

        // GET: ClienteController
        public ActionResult Index()
        {
            var listaClientes = _clienteService.GetAll();
            var listaClientesViewModel = _mapper.Map<List<ClienteViewModel>>(listaClientes);
            return View(listaClientesViewModel);
        }

        // GET: ClienteController/Perfil
        // Redireciona o cliente logado para o seu próprio Details
        public async Task<ActionResult> Perfil()
        {
            // Tenta buscar o ID do cliente da sessão primeiro
            var clienteId = HttpContext.Session.GetClienteId();

            if (clienteId.HasValue)
            {
                return RedirectToAction(nameof(Details), new { id = clienteId.Value });
            }

            // Se não estiver na sessão, busca pelo email e armazena
            var userEmail = _userManager.GetUserName(User);
            var cliente = _clienteService.GetByEmail(userEmail);

            if (cliente == null)
                return NotFound("Perfil de cliente não encontrado para o usuário logado.");

            // Armazena na sessão para próximas requisições
            HttpContext.Session.SetClienteId(cliente.Id);

            return RedirectToAction(nameof(Details), new { id = cliente.Id });
        }

        // GET: ClienteController/Details/5
        public ActionResult Details(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteModel);
        }

        // GET: ClienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(ClienteViewModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                // Validação e normalização do telefone
                if (clienteModel == null || string.IsNullOrWhiteSpace(clienteModel.Telefone))
                {
                    ModelState.AddModelError("Telefone", "Telefone inválido.");
                    return View(clienteModel);
                }

                var normalized = new string(clienteModel.Telefone.Where(char.IsDigit).ToArray());
                if (normalized.Length != 11)
                {
                    ModelState.AddModelError("Telefone", "O telefone deve conter 11 dígitos.");
                    return View(clienteModel);
                }

                // Atualiza o modelo com o telefone normalizado (somente dígitos)
                clienteModel.Telefone = normalized;

                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Create(cliente);

                return RedirectToAction(nameof(Index));
            }

            return View(clienteModel);
        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteModel);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteViewModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                // Validação e normalização do telefone antes de salvar
                if (clienteModel == null || string.IsNullOrWhiteSpace(clienteModel.Telefone))
                {
                    ModelState.AddModelError("Telefone", "Telefone inválido.");
                    return View(clienteModel);
                }

                var normalized = new string(clienteModel.Telefone.Where(char.IsDigit).ToArray());
                if (normalized.Length != 11)
                {
                    ModelState.AddModelError("Telefone", "O telefone deve conter 11 dígitos.");
                    return View(clienteModel);
                }

                // Atualiza o modelo com o telefone normalizado
                clienteModel.Telefone = normalized;

                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Edit(cliente);
                return RedirectToAction(nameof(Index));
            }

            return View(clienteModel);
        }

        // GET: ClienteController/Delete/5
        public ActionResult Delete(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteModel);
        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ClienteViewModel clienteModel)
        {
            // Busca o cliente para obter o email
            var cliente = _clienteService.Get(clienteModel.Id);

            if (cliente != null && !string.IsNullOrEmpty(cliente.Email))
            {
                // Remove o usuário do Identity primeiro
                var user = await _userManager.FindByEmailAsync(cliente.Email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            // Remove o cliente do banco delifit
            _clienteService.Delete(clienteModel.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: ClienteController/HomeCliente
        // Página inicial do cliente logado
        [Authorize(Roles = "Cliente")]
        public ActionResult HomeCliente()
        {
            var clienteId = GetClienteIdLogado();

            if (!clienteId.HasValue)
            {
                TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
                return RedirectToAction("Index", "Home");
            }

            // Busca informações do cliente
            var cliente = _clienteService.Get(clienteId.Value);
            ViewBag.NomeCliente = cliente?.Nome;

            // Busca restaurantes ativos
            var restaurantesAtivos = _restauranteService.GetRestaurantesAtivos();
            var restaurantesViewModel = _mapper.Map<List<RestauranteViewModel>>(restaurantesAtivos);

            return View(restaurantesViewModel);
        }

        // Método auxiliar para obter o ID do cliente logado
        private uint? GetClienteIdLogado()
        {
            // Tenta buscar da sessão
            var clienteId = HttpContext.Session.GetClienteId();

            if (!clienteId.HasValue)
            {
                // Se não estiver na sessão, busca pelo email
                var userEmail = _userManager.GetUserName(User);
                var cliente = _clienteService.GetByEmail(userEmail);

                if (cliente != null)
                {
                    // Armazena na sessão para próximas requisições
                    HttpContext.Session.SetClienteId(cliente.Id);
                    clienteId = cliente.Id;
                }
            }

            return clienteId;
        }
    }
}
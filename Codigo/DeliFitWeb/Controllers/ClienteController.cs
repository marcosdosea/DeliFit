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
        private readonly IItemService _itemService;

        public ClienteController(IClienteService clienteService,
                                 IMapper mapper,
                                 UserManager<UsuarioIdentity> userManager,
                                 IRestauranteService restauranteService,
                                 IItemService itemService)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _userManager = userManager;
            _restauranteService = restauranteService;
            _itemService = itemService;
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

            var userEmail = _userManager.GetUserName(User);
            var cliente = _clienteService.GetByEmail(userEmail);

            if (cliente == null)
                return NotFound("Perfil de cliente não encontrado para o usuário logado.");

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
            var cliente = _clienteService.Get(clienteModel.Id);

            if (cliente != null && !string.IsNullOrEmpty(cliente.Email))
            {
                var user = await _userManager.FindByEmailAsync(cliente.Email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            _clienteService.Delete(clienteModel.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: ClienteController/HomeCliente
        [Authorize(Roles = "Cliente")]
        public ActionResult HomeCliente()
        {
            var clienteId = GetClienteIdLogado();

            if (!clienteId.HasValue)
            {
                TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
                return RedirectToAction("Index", "Home");
            }

            var cliente = _clienteService.Get(clienteId.Value);
            ViewBag.NomeCliente = cliente?.Nome;

            // Restaurantes ativos
            var restaurantesAtivos = _restauranteService.GetRestaurantesAtivos().ToList();
            var restaurantesViewModel = _mapper.Map<List<RestauranteViewModel>>(restaurantesAtivos);

            // Itens de todos os restaurantes ativos
            var todosItens = new List<ItemViewModel>();
            foreach (var r in restaurantesAtivos)
            {
                var itens = _itemService.GetByRestaurante(r.Id);
                todosItens.AddRange(_mapper.Map<List<ItemViewModel>>(itens));
            }
            ViewBag.Itens = todosItens;

            // Endereço principal do cliente para exibição no header
            ViewBag.ClienteId = clienteId.Value;

            return View(restaurantesViewModel);
        }

        private uint? GetClienteIdLogado()
        {
            var clienteId = HttpContext.Session.GetClienteId();

            if (!clienteId.HasValue)
            {
                var userEmail = _userManager.GetUserName(User);
                var cliente = _clienteService.GetByEmail(userEmail);

                if (cliente != null)
                {
                    HttpContext.Session.SetClienteId(cliente.Id);
                    clienteId = cliente.Id;
                }
            }

            return clienteId;
        }
    }
}
using AutoMapper;
using Core;
using Core.Identity.Data;
using Core.Service;
using DeliFitWeb.Helpers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliFitWeb.Controllers;

public class ClienteController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IMapper _mapper;
    private readonly UserManager<UsuarioIdentity> _userManager;
    private readonly IRestauranteService _restauranteService;
    private readonly IItemService _itemService;
    private readonly ICategoriaService _categoriaService;
    private readonly ICarrinhoService _carrinhoService;
    private readonly IPedidoService _pedidoService;
    private readonly IAvaliacaoService _avaliacaoService;

    public ClienteController(IClienteService clienteService,
                             IMapper mapper,
                             UserManager<UsuarioIdentity> userManager,
                             IRestauranteService restauranteService,
                             IItemService itemService,
                             ICategoriaService categoriaService,
                             ICarrinhoService carrinhoService,
                             IPedidoService pedidoService,
                             IAvaliacaoService avaliacaoService)
    {
        _clienteService = clienteService;
        _mapper = mapper;
        _userManager = userManager;
        _restauranteService = restauranteService;
        _itemService = itemService;
        _categoriaService = categoriaService;
        _carrinhoService = carrinhoService;
        _pedidoService = pedidoService;
        _avaliacaoService = avaliacaoService;
    }

    // GET: ClienteController
    public ActionResult Index()
    {
        var listaClientes = _clienteService.GetAll();
        var listaClientesViewModel = _mapper.Map<List<ClienteViewModel>>(listaClientes);
        return View(listaClientesViewModel);
    }

    // GET: ClienteController/Perfil
    public async Task<ActionResult> Perfil()
    {
        var clienteId = HttpContext.Session.GetClienteId();

        if (clienteId.HasValue)
            return RedirectToAction(nameof(Details), new { id = clienteId.Value });

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
        if (cliente == null)
        {
            TempData["Error"] = "Cliente não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
        return View(clienteModel);
    }

    // GET: ClienteController/Create
    public ActionResult Create()
    {
        return View(new ClienteViewModel());
    }

    // POST: ClienteController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ClienteViewModel clienteModel)
    {
        if (ModelState.IsValid)
        {
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
        if (cliente == null)
        {
            TempData["Error"] = "Cliente não encontrado.";
            return RedirectToAction(nameof(Index));
        }

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
            if (string.IsNullOrWhiteSpace(clienteModel.Telefone))
            {
                ModelState.AddModelError("Telefone", "O telefone é obrigatório.");
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
        if (cliente == null)
        {
            TempData["Error"] = "Cliente não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
        return View(clienteModel);
    }

    // POST: ClienteController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(ClienteViewModel clienteModel)
    {
        var cliente = _clienteService.Get(clienteModel.Id);
        if (cliente == null)
        {
            TempData["Error"] = "Cliente não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _clienteService.Delete(clienteModel.Id);
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível desativar este cliente.";
            return RedirectToAction(nameof(Details), new { id = clienteModel.Id });
        }

        // Remove o login (Identity) depois que o cliente foi desativado com sucesso,
        // para não deixar a conta órfã se a desativação falhar. O registro do cliente
        // em si permanece no banco (soft delete) para preservar o histórico de pedidos.
        if (!string.IsNullOrEmpty(cliente.Email))
        {
            var user = await _userManager.FindByEmailAsync(cliente.Email);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        TempData["Success"] = "Cliente desativado com sucesso.";
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

        // Restaurantes ativos
        var restaurantesAtivos = _restauranteService.GetRestaurantesAtivos().ToList();
        var restaurantesViewModel = _mapper.Map<List<RestauranteViewModel>>(restaurantesAtivos);

        // Média real de avaliações por restaurante
        var todosPedidos = _pedidoService.GetAll().ToList();
        var todasAvaliacoes = _avaliacaoService.GetAll().ToList();
        var pedidosPorRestaurante = todosPedidos
            .GroupBy(p => p.IdRestaurante)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Id).ToHashSet());

        foreach (var vm in restaurantesViewModel)
        {
            if (pedidosPorRestaurante.TryGetValue(vm.Id, out var pedidoIds))
            {
                var notas = todasAvaliacoes
                    .Where(a => pedidoIds.Contains(a.IdPedido))
                    .Select(a => (double)a.Nota)
                    .ToList();
                vm.MediaAvaliacao = notas.Any() ? notas.Average() : 0.0;
            }
        }

        // Itens de todos os restaurantes ativos
        var restauranteIds = restaurantesAtivos.Select(r => r.Id).ToHashSet();
        var itensRestaurantesAtivos = _itemService
            .GetAll()
            .Where(i => restauranteIds.Contains(i.IdRestaurante))
            .ToList();
        ViewBag.Itens = _mapper.Map<List<ItemViewModel>>(itensRestaurantesAtivos);

        // Categorias dinâmicas do banco
        var categorias = _categoriaService.ListarCategorias().ToList();
        ViewBag.Categorias = _mapper.Map<List<CategoriaViewModel>>(categorias);

        ViewBag.ClienteId = clienteId.Value;

        return View(restaurantesViewModel);
    }

    // GET: ClienteController/Consumo — CSU20 Gerenciar Consumo Calórico
    [Authorize(Roles = "Cliente")]
    public ActionResult Consumo(string? data = null)
    {
        var clienteId = GetClienteIdLogado();
        if (!clienteId.HasValue)
        {
            TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
            return RedirectToAction("Index", "Home");
        }

        var dataSelecionada = data != null && DateTime.TryParse(data, out var parsedDate)
            ? parsedDate.Date
            : DateTime.Today;

        var carrinhoIds = _carrinhoService.GetAll()
            .Where(c => c.IdCliente == clienteId.Value)
            .Select(c => c.Id)
            .ToHashSet();

        var pedidosFinalizados = _pedidoService.GetAll()
            .Where(p => carrinhoIds.Contains(p.IdCarrinho) && p.Status == 'F' && p.Data.HasValue)
            .ToList();

        var datasComPedidos = pedidosFinalizados
            .Select(p => p.Data!.Value.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var pedidosDoDia = pedidosFinalizados
            .Where(p => p.Data!.Value.Date == dataSelecionada)
            .Select(p => _pedidoService.Get(p.Id))
            .Where(p => p != null)
            .ToList();

        var itensDoDia = pedidosDoDia
            .SelectMany(p => p!.Pedidoitems)
            .Where(pi => pi.IdItemNavigation != null)
            .Select(pi => pi.IdItemNavigation!)
            .ToList();

        ViewBag.DataSelecionada = dataSelecionada;
        ViewBag.DatasComPedidos = datasComPedidos;
        ViewBag.TotalKcal = itensDoDia.Sum(i => (double?)i.Calorias) ?? 0;
        ViewBag.TotalProteina = itensDoDia.Sum(i => (double?)i.Proteina) ?? 0;
        ViewBag.TotalCarboidratos = itensDoDia.Sum(i => (double?)i.Carboidratos) ?? 0;
        ViewBag.TotalGordura = itensDoDia.Sum(i => (double?)i.Gordura) ?? 0;

        return View(itensDoDia);
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

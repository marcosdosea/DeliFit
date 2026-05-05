using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using DeliFitWeb.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DeliFitWeb.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IMapper _mapper;
        private readonly IClienteService _clienteService;
        private readonly ICarrinhoService _carrinhoService;
        private readonly IRestauranteService _restauranteService;

        public PedidoController(IPedidoService pedidoService, IMapper mapper, IClienteService clienteService, ICarrinhoService carrinhoService, IRestauranteService restauranteService)
        {
            _pedidoService = pedidoService;
            _mapper = mapper;
            _clienteService = clienteService;
            _carrinhoService = carrinhoService;
            _restauranteService = restauranteService;
        }

        // GET: PedidoController
        [Authorize(Roles = "Cliente,Admin")]
        public ActionResult Index()
        {
            IEnumerable<Pedido> listaPedidos;

            if (User.IsInRole("Cliente"))
            {
                // Cliente vê apenas seus próprios pedidos
                var clienteId = GetClienteIdLogado();
                if (clienteId.HasValue)
                {
                    // Filtra pelos carrinhos do cliente e depois pelos pedidos desses carrinhos
                    // Evita acessar IdCarrinhoNavigation que pode ser null (não carregado pelo EF)
                    var carrinhoIds = _carrinhoService.GetAll()
                        .Where(c => c.IdCliente == clienteId.Value)
                        .Select(c => c.Id)
                        .ToHashSet();

                    listaPedidos = _pedidoService.GetAll()
                        .Where(p => carrinhoIds.Contains(p.IdCarrinho));
                }
                else
                {
                    TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Admin vê todos os pedidos
                listaPedidos = _pedidoService.GetAll();
            }

            var listaPedidosViewModel = _mapper.Map<List<PedidoViewModel>>(listaPedidos);
            return View(listaPedidosViewModel);
        }

        // GET: PedidoController/Details/5
        public ActionResult Details(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            PedidoViewModel pedidoViewModel = _mapper.Map<PedidoViewModel>(pedido);
            return View(pedidoViewModel);
        }

        // GET: PedidoController/Acompanhar/5 — Tela M16
        public ActionResult Acompanhar(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            if (pedido == null)
                return RedirectToAction(nameof(Index));

            PedidoViewModel pedidoViewModel = _mapper.Map<PedidoViewModel>(pedido);

            // Carrega nome do restaurante
            ViewBag.NomeRestaurante = _restauranteService != null
                ? _restauranteService.Get(pedido.IdRestaurante)?.NomeRestaurante ?? ""
                : "";

            // Carrega itens do pedido
            ViewBag.Itens = pedido.Pedidoitems?.ToList() ?? new List<Pedidoitem>();

            // Carrega forma de pagamento do carrinho
            ViewBag.FormaPagamento = "";

            return View(pedidoViewModel);
        }

        // GET: PedidoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PedidoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public ActionResult Create(PedidoViewModel pedidoModel)
        {
            // Nota: Pedido requer IdCarrinho, não IdCliente diretamente
            // O carrinho já deve existir e estar vinculado ao cliente

            if (ModelState.IsValid)
            {
                try
                {
                    var pedido = _mapper.Map<Pedido>(pedidoModel);
                    _pedidoService.Create(pedido);
                    TempData["Success"] = "Pedido criado com sucesso!";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar pedido: {ex.Message}");
                    return View(pedidoModel);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PedidoController/Delete/5
        public ActionResult Delete(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            PedidoViewModel pedidoModel = _mapper.Map<PedidoViewModel>(pedido);

            return View(pedidoModel);
        }

        // POST: PedidoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PedidoViewModel pedidoModel)
        {
            _pedidoService.Delete(pedidoModel.Id);
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para obter o ID do cliente logado
        private uint? GetClienteIdLogado()
        {
            // Tenta buscar da sessão
            var clienteId = HttpContext.Session.GetClienteId();

            if (!clienteId.HasValue)
            {
                // Se não estiver na sessão, busca pelo email
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var cliente = _clienteService.GetByEmail(userEmail);

                    if (cliente != null)
                    {
                        // Armazena na sessão para próximas requisições
                        HttpContext.Session.SetClienteId(cliente.Id);
                        clienteId = cliente.Id;
                    }
                }
            }

            return clienteId;
        }
    }
}

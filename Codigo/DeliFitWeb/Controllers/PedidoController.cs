using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IAvaliacaoService _avaliacaoService;

        public PedidoController(
            IPedidoService pedidoService,
            IMapper mapper,
            IClienteService clienteService,
            ICarrinhoService carrinhoService,
            IRestauranteService restauranteService,
            IAvaliacaoService avaliacaoService)
        {
            _pedidoService = pedidoService;
            _mapper = mapper;
            _clienteService = clienteService;
            _carrinhoService = carrinhoService;
            _restauranteService = restauranteService;
            _avaliacaoService = avaliacaoService;
        }

        // GET: PedidoController
        [Authorize(Roles = "Cliente,Admin")]
        public ActionResult Index()
        {
            IEnumerable<Pedido> listaPedidos;

            if (User.IsInRole("Cliente"))
            {
                var clienteId = GetClienteIdLogado();
                if (clienteId.HasValue)
                {
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
                listaPedidos = _pedidoService.GetAll();
            }

            var listaPedidosViewModel = _mapper.Map<List<PedidoViewModel>>(listaPedidos);
            return View(listaPedidosViewModel);
        }

        // GET: PedidoController/Details/5
        public ActionResult Details(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            if (pedido == null)
                return RedirectToAction(nameof(Index));

            PedidoViewModel pedidoViewModel = _mapper.Map<PedidoViewModel>(pedido);

            ViewBag.NomeRestaurante = _restauranteService?.Get(pedido.IdRestaurante)?.NomeRestaurante ?? "";
            ViewBag.Itens = pedido.Pedidoitems?.ToList() ?? new List<Pedidoitem>();

            var carrinho = _carrinhoService.Get(pedido.IdCarrinho);
            if (carrinho != null)
            {
                var cliente = _clienteService.Get(carrinho.IdCliente);
                if (cliente != null)
                {
                    ViewBag.NomeCliente = cliente.Nome ?? "";
                    ViewBag.TelefoneCliente = cliente.Telefone ?? "";
                    ViewBag.EnderecoCliente = cliente.Enderecos?.FirstOrDefault()?.Rua ?? "Não informado";
                    ViewBag.NumeroCliente = cliente.Enderecos?.FirstOrDefault()?.Numero ?? "";
                    ViewBag.BairroCliente = cliente.Enderecos?.FirstOrDefault()?.Bairro ?? "";
                }
            }

            ViewBag.StatusPedido = pedido.Status ?? 'P';

            return View(pedidoViewModel);
        }

        // GET: PedidoController/Acompanhar/5 — Tela M16
        [Authorize(Roles = "Cliente")]
        public ActionResult Acompanhar(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            if (pedido == null)
                return RedirectToAction(nameof(Index));

            PedidoViewModel pedidoViewModel = _mapper.Map<PedidoViewModel>(pedido);

            ViewBag.NomeRestaurante = _restauranteService?.Get(pedido.IdRestaurante)?.NomeRestaurante ?? "";
            ViewBag.Itens = pedido.Pedidoitems?.ToList() ?? new List<Pedidoitem>();

            var carrinho = _carrinhoService.Get(pedido.IdCarrinho);
            ViewBag.FormaPagamento = carrinho?.FormaDePagamento ?? "";

            ViewBag.StatusPedido = pedido.Status ?? 'P';

            // Verifica se o pedido já foi avaliado — impede dupla avaliação
            ViewBag.AvaliacaoExistente = _avaliacaoService.GetByPedido(id);

            return View(pedidoViewModel);
        }

        // GET: PedidoController/GetStatus/5
        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public IActionResult GetStatus(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            if (pedido == null)
                return NotFound();

            return Json(new { status = pedido.Status?.ToString() ?? "P" });
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
            if (ModelState.IsValid)
            {
                try
                {
                    var pedido = _mapper.Map<Pedido>(pedidoModel);
                    _pedidoService.Create(pedido);
                    TempData["Success"] = "Pedido criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Em caso de exceção, mantemos o comportamento de retornar a View para correção
                    ModelState.AddModelError("", $"Erro ao criar pedido: {ex.Message}");
                    return View(pedidoModel);
                }
            }

            // Se inválido, retornamos a View para que o usuário corrija os dados (testes esperam ViewResult quando ModelState inválido)
            return View(pedidoModel);
        }

        // GET: PedidoController/Delete/5
        public ActionResult Delete(uint id)
        {
            Pedido? pedido = _pedidoService.Get(id);
            if (pedido == null)
            {
                TempData["Error"] = "Pedido não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            PedidoViewModel pedidoModel = _mapper.Map<PedidoViewModel>(pedido);
            return View(pedidoModel);
        }

        // POST: PedidoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PedidoViewModel pedidoModel)
        {
            try
            {
                _pedidoService.Delete(pedidoModel.Id);
            }
            catch (ServiceException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Não é possível excluir este pedido pois existem avaliações vinculadas a ele.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteAjax([FromBody] uint id)
        {
            try
            {
                _pedidoService.Delete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private uint? GetClienteIdLogado()
        {
            var clienteId = HttpContext.Session.GetClienteId();

            if (!clienteId.HasValue)
            {
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var cliente = _clienteService.GetByEmail(userEmail);
                    if (cliente != null)
                    {
                        HttpContext.Session.SetClienteId(cliente.Id);
                        clienteId = cliente.Id;
                    }
                }
            }

            return clienteId;
        }
    }
}
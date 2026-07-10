using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Helpers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Core.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class AvaliacaoController : Controller
    {
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly ICarrinhoService _carrinhoService;
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public AvaliacaoController(
            IAvaliacaoService avaliacaoService,
            IPedidoService pedidoService,
            IClienteService clienteService,
            ICarrinhoService carrinhoService,
            IRestauranteService restauranteService,
            IMapper mapper,
            UserManager<UsuarioIdentity> userManager)
        {
            _avaliacaoService = avaliacaoService;
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _carrinhoService = carrinhoService;
            _restauranteService = restauranteService;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: /Avaliacao/Avaliar/5 — Tela_M17
        [HttpGet]
        public ActionResult Avaliar(uint idPedido)
        {
            var pedido = _pedidoService.Get(idPedido);
            if (pedido == null)
                return RedirectToAction("Index", "Pedido");

            var clienteId = GetClienteIdLogado();
            if (!clienteId.HasValue)
                return RedirectToAction("Index", "Home");

            if (!PedidoPertenceAoCliente(pedido, clienteId.Value))
            {
                TempData["Error"] = "Este pedido não pertence à sua conta.";
                return RedirectToAction("Index", "Pedido");
            }

            var restaurante = _restauranteService.Get(pedido.IdRestaurante);
            ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
            ViewBag.IdRestaurante = pedido.IdRestaurante;

            // Passa os itens do pedido para o resumo na M17
            ViewBag.Itens = pedido.Pedidoitems?.ToList() ?? new List<Pedidoitem>();

            var model = new AvaliacaoViewModel
            {
                IdPedido = idPedido,
                IdCliente = clienteId.Value
            };

            return View(model);
        }

        // POST: /Avaliacao/Avaliar — Tela_M17
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Avaliar(AvaliacaoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var pedido = _pedidoService.Get(model.IdPedido);
                var restaurante = pedido != null ? _restauranteService.Get(pedido.IdRestaurante) : null;
                ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
                ViewBag.IdRestaurante = pedido?.IdRestaurante ?? 0;
                ViewBag.Itens = pedido?.Pedidoitems?.ToList() ?? new List<Pedidoitem>();
                TempData["Error"] = "Falha no envio da avaliação!";
                return View(model);
            }

            var pedidoParaAvaliar = _pedidoService.Get(model.IdPedido);
            var clienteIdLogado = GetClienteIdLogado();

            if (pedidoParaAvaliar == null || !clienteIdLogado.HasValue ||
                !PedidoPertenceAoCliente(pedidoParaAvaliar, clienteIdLogado.Value))
            {
                TempData["Error"] = "Este pedido não pertence à sua conta.";
                return RedirectToAction("Index", "Pedido");
            }

            try
            {
                var avaliacao = new Avaliacao
                {
                    IdPedido = model.IdPedido,
                    IdCliente = clienteIdLogado.Value,
                    Nota = model.Nota,
                    Descricao = model.Descricao ?? string.Empty
                };

                _avaliacaoService.Create(avaliacao);
                TempData["Success"] = "Avaliação enviada com sucesso!";
                return RedirectToAction("HomeCliente", "Cliente");
            }
            catch
            {
                var restaurante = _restauranteService.Get(pedidoParaAvaliar.IdRestaurante);
                ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
                ViewBag.IdRestaurante = pedidoParaAvaliar.IdRestaurante;
                ViewBag.Itens = pedidoParaAvaliar.Pedidoitems?.ToList() ?? new List<Pedidoitem>();
                TempData["Error"] = "Falha no envio da avaliação!";
                return View(model);
            }
        }

        // GET: /Avaliacao/Reclamacao/5 — Tela_M21
        [HttpGet]
        public ActionResult Reclamacao(uint idPedido)
        {
            var pedido = _pedidoService.Get(idPedido);
            if (pedido == null)
                return RedirectToAction("Index", "Pedido");

            var clienteId = GetClienteIdLogado();
            if (!clienteId.HasValue)
                return RedirectToAction("Index", "Home");

            if (!PedidoPertenceAoCliente(pedido, clienteId.Value))
            {
                TempData["Error"] = "Este pedido não pertence à sua conta.";
                return RedirectToAction("Index", "Pedido");
            }

            var restaurante = _restauranteService.Get(pedido.IdRestaurante);
            ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";

            var model = new ReclamacaoViewModel { IdPedido = idPedido };
            return View(model);
        }

        // POST: /Avaliacao/Reclamacao — Tela_M21
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reclamacao(ReclamacaoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var pedido = _pedidoService.Get(model.IdPedido);
                var restaurante = pedido != null ? _restauranteService.Get(pedido.IdRestaurante) : null;
                ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
                TempData["Error"] = "Ocorreu uma falha durante o envio da reclamação!";
                return View(model);
            }

            try
            {
                var pedidoParaReclamar = _pedidoService.Get(model.IdPedido);
                var clienteId = GetClienteIdLogado();

                if (pedidoParaReclamar == null || !clienteId.HasValue ||
                    !PedidoPertenceAoCliente(pedidoParaReclamar, clienteId.Value))
                {
                    TempData["Error"] = "Este pedido não pertence à sua conta.";
                    return RedirectToAction("Index", "Pedido");
                }

                // A descrição já vem prefixada pelo JS com o motivo selecionado,
                // mas garantimos o prefixo [RECLAMAÇÃO] para rastreabilidade no banco.
                var descricaoFinal = model.Descricao.StartsWith("[RECLAMAÇÃO]")
                    ? model.Descricao
                    : $"[RECLAMAÇÃO] {model.Descricao}";

                var avaliacao = new Avaliacao
                {
                    IdPedido = model.IdPedido,
                    IdCliente = clienteId.Value,
                    Nota = 1,
                    Descricao = descricaoFinal
                };

                _avaliacaoService.Create(avaliacao);
                TempData["Success"] = "Reclamação enviada com sucesso!";
                return RedirectToAction("HomeCliente", "Cliente");
            }
            catch
            {
                var pedido = _pedidoService.Get(model.IdPedido);
                var restaurante = pedido != null ? _restauranteService.Get(pedido.IdRestaurante) : null;
                ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
                TempData["Error"] = "Ocorreu uma falha durante o envio da reclamação!";
                return View(model);
            }
        }

        private bool PedidoPertenceAoCliente(Pedido pedido, uint clienteId)
        {
            var carrinho = _carrinhoService.Get(pedido.IdCarrinho);
            return carrinho != null && carrinho.IdCliente == clienteId;
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
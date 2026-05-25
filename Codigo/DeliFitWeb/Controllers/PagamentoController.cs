using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Helpers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers
{
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;

        public PagamentoController(IPagamentoService pagamentoService,
                                   IRestauranteService restauranteService,
                                   IMapper mapper)
        {
            _pagamentoService = pagamentoService;
            _restauranteService = restauranteService;
            _mapper = mapper;
        }

        // GET: Pagamento/Index
        public ActionResult Index()
        {
            if (User.IsInRole("GerenteRestaurante"))
            {
                var restauranteId = HttpContext.Session.GetRestauranteId();
                if (!restauranteId.HasValue)
                {
                    var userEmail = User.Identity?.Name;
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var rest = _restauranteService.GetByEmail(userEmail);
                        if (rest != null)
                        {
                            HttpContext.Session.SetRestauranteId(rest.Id);
                            restauranteId = rest.Id;
                        }
                    }
                }

                if (!restauranteId.HasValue)
                {
                    TempData["Error"] = "Não foi possível identificar o restaurante.";
                    return RedirectToAction("Home", "Restaurante");
                }

                return RedirectToAction(nameof(PorRestaurante), new { idRestaurante = restauranteId.Value });
            }

            // Admin: mostra todos
            var listaPagamentos = _pagamentoService.GetAll();
            var listaPagamentosViewModel = _mapper.Map<List<PagamentoViewModel>>(listaPagamentos);
            ViewBag.Titulo = "Todos os Pagamentos de Mensalidade";
            ViewBag.NomeRestaurante = "";
            return View(listaPagamentosViewModel);
        }

        // GET: Pagamento/PorRestaurante/5
        public ActionResult PorRestaurante(uint idRestaurante)
        {
            var pagamentos = _pagamentoService.GetAllByRestaurante(idRestaurante);
            var viewModel = _mapper.Map<List<PagamentoViewModel>>(pagamentos);

            var restaurante = _restauranteService.Get(idRestaurante);
            ViewBag.NomeRestaurante = restaurante?.NomeRestaurante ?? "";
            ViewBag.Titulo = $"Mensalidades";
            return View("Index", viewModel);
        }

        // GET: PagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            Pagamento? pagamento = _pagamentoService.Get(id);
            PagamentoViewModel pagamentoModel = _mapper.Map<PagamentoViewModel>(pagamento);
            return View(pagamentoModel);
        }

        // GET: PagamentoController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(PagamentoViewModel pagamentoModel)
        {
            if (ModelState.IsValid)
            {
                var pagamento = _mapper.Map<Pagamento>(pagamentoModel);
                _pagamentoService.Create(pagamento);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult PorStatus(string status)
        {
            var pagamentos = _pagamentoService.GetAllByStatus(status);
            var viewModel = _mapper.Map<List<PagamentoViewModel>>(pagamentos);
            ViewBag.Titulo = $"Pagamentos — Status: {status}";
            ViewBag.NomeRestaurante = "";
            return View("Index", viewModel);
        }
    }
}

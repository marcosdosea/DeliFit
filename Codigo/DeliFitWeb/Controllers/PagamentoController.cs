using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
namespace DeliFitWeb.Controllers
{
    public class PagamentoController : Controller
    {

        private readonly IPagamentoService _pagamentoService;
        private readonly IMapper _mapper;

        public PagamentoController(IPagamentoService pagamentoService, IMapper mapper)
        {
            _pagamentoService = pagamentoService;
            _mapper = mapper;
        }

        // GET: PagamentoController
        public ActionResult Index()
        {
            var listaPagamentos = _pagamentoService.GetAll();
            var listaPagamentosViewModel = _mapper.Map<List<PagamentoViewModel>>(listaPagamentos);

            return View(listaPagamentosViewModel);
        }

        // GET: PagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            Pagamento? pagamento = _pagamentoService.Get(id);
            PagamentoViewModel pagamentoModel = _mapper.Map<PagamentoViewModel>(pagamento);
            return View(pagamentoModel);
        }

        // GET: PagamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PagamentoViewModel pagamentoModel)
        {
            if (ModelState.IsValid)
            {
                var pagamento = _mapper.Map<Pagamento>(pagamentoModel);
                _pagamentoService.Create(pagamento);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult PorRestaurante(uint idRestaurante)
        {
            var pagamentos = _pagamentoService.GetAllByRestaurante(idRestaurante);
            var viewModel = _mapper.Map<List<PagamentoViewModel>>(pagamentos);

            return View("Index", viewModel);
        }

        public ActionResult PorStatus(string status)
        {
            var pagamentos = _pagamentoService.GetAllByStatus(status);
            var viewModel = _mapper.Map<List<PagamentoViewModel>>(pagamentos);

            return View("Index", viewModel); //usando para preservar os filtros feitos
        }






    }
}

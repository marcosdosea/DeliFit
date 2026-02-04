using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
namespace DeliFitWeb.Controllers
{
    public class PedidoController : Controller
    {

        private readonly IPedidoService _pedidoService;
        private readonly IMapper _mapper;

        public PedidoController(IPedidoService pedidoService, IMapper mapper)
        {
            _pedidoService = pedidoService;
            _mapper = mapper;
           
        }

        // GET: PedidoController
        public ActionResult Index()
        {
            var listaPedidos = _pedidoService.GetAll();
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

        // GET: PedidoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PedidoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoViewModel pedidoModel)
        {
            if (ModelState.IsValid)
            {
                var pedido = _mapper.Map<Pedido>(pedidoModel);
                _pedidoService.Create(pedido);
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
        
            
        
    }
}

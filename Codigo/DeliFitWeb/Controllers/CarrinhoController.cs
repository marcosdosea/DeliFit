using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;

namespace DeliFitWeb.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly IClienteService _clienteService;
        private readonly ICartaoService _cartaoService;

        private readonly IMapper _mapper;

        public CarrinhoController(ICarrinhoService carrinhoService, 
                                    IClienteService clienteService, 
                                    ICartaoService cartaoService ,
                                    IMapper mapper)
        {
            _clienteService = clienteService;
            _cartaoService = cartaoService;
            _carrinhoService = carrinhoService;
            _mapper = mapper;
        }

        public void CarregarDados()
        {
            ViewBag.Clientes = _clienteService.GetAll()
                .Select(c => new { c.Id, c.Nome })
                .ToList();
            ViewBag.Cartoes = _cartaoService.GetAll()
                .Select(c => new { c.Id, c.Numero , c.IdCliente})
                .ToList();

        }

        public ActionResult Index()
        {
            var carrinhos = _carrinhoService.GetAll();
            var viewModels = _mapper.Map<List<CarrinhoViewModel>>(carrinhos);
            return View(viewModels);
        }

        public ActionResult Details(uint id)
        {
            var carrinho = _carrinhoService.Get(id);
            var viewModel = _mapper.Map<CarrinhoViewModel>(carrinho);
            return View(viewModel);
        }

        public ActionResult Create()
        {
            CarregarDados();
            return View(new CarrinhoViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CarrinhoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                CarregarDados();
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var carrinho = _mapper.Map<Carrinho>(viewModel);
                _carrinhoService.Create(carrinho);
                return RedirectToAction(nameof(Index));
                
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CarregarDados();
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Delete(uint id)
        {
            var carrinho = _carrinhoService.Get(id);
            if (carrinho == null)
                return NotFound();

            var viewModel = _mapper.Map<CarrinhoViewModel>(carrinho);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CarrinhoViewModel viewModel)
        {
            _carrinhoService.Delete(viewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}

using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DeliFitWeb.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly IMapper _mapper;

        public CarrinhoController(ICarrinhoService carrinhoService, IMapper mapper)
        {
            _carrinhoService = carrinhoService;
            _mapper = mapper;
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


        //NAO PRECISA DESSES EU ACHO :/

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(CarrinhoViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var carrinho = _mapper.Map<Carrinho>(viewModel);
        //        _carrinhoService.Create(carrinho);
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        //public ActionResult Edit(uint id)
        //{
        //    var carrinho = _carrinhoService.Get(id);
        //    var viewModel = _mapper.Map<CarrinhoViewModel>(carrinho);
        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(CarrinhoViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var carrinho = _mapper.Map<Carrinho>(viewModel);
        //        _carrinhoService.Edit(carrinho);
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        //public ActionResult Delete(uint id)
        //{
        //    var carrinho = _carrinhoService.Get(id);
        //    var viewModel = _mapper.Map<CarrinhoViewModel>(carrinho);
        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(CarrinhoViewModel viewModel)
        //{
        //    _carrinhoService.Delete(viewModel.Id);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}

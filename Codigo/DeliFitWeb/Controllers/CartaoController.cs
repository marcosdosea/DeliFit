using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DeliFitWeb.Controllers
{
    public class CartaoController : Controller
    {
        private readonly ICartaoService _cartaoService;
        private readonly IMapper _mapper;

        public CartaoController(ICartaoService cartaoService, IMapper mapper)
        {
            _cartaoService = cartaoService;
            _mapper = mapper;
        }

        public ActionResult Index(uint idCliente)
        {
            var listaCartoes = _cartaoService.GetByCliente(idCliente);
            var listaViewModel = _mapper.Map<List<CartaoViewModel>>(listaCartoes);
            return View(listaViewModel);
        }


        public ActionResult Details(uint id)
        {
            var cartao = _cartaoService.Get(id);
            var viewModel = _mapper.Map<CartaoViewModel>(cartao);
            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CartaoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var cartao = _mapper.Map<Cartao>(viewModel);
                _cartaoService.Create(cartao);
            }
            return RedirectToAction(nameof(Index), new { idCliente = viewModel.IdCliente });
        }

        public ActionResult Delete(uint id)
        {
            var cartao = _cartaoService.Get(id);
            var viewModel = _mapper.Map<CartaoViewModel>(cartao);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CartaoViewModel viewModel)
        {
            _cartaoService.Delete(viewModel.Id);
            return RedirectToAction(nameof(Index), new { idCliente = viewModel.IdCliente });

        }
    }
}

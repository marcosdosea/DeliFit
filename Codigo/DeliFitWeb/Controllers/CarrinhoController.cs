using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Helpers;

namespace DeliFitWeb.Controllers;

public class CarrinhoController : Controller
{
    private readonly ICarrinhoService _carrinhoService;
    private readonly IClienteService _clienteService;
    private readonly ICartaoService _cartaoService;

    private readonly IMapper _mapper;

    public CarrinhoController(ICarrinhoService carrinhoService,
                                IClienteService clienteService,
                                ICartaoService cartaoService,
                                IMapper mapper)
    {
        _clienteService = clienteService;
        _cartaoService = cartaoService;
        _carrinhoService = carrinhoService;
        _mapper = mapper;
    }

    public void CarregarDados()
    {
        var clientes = _clienteService.GetAll();
        ViewBag.Clientes = _mapper.Map<List<ClienteViewModel>>(clientes);

        var cartoes = _cartaoService.GetAll();
        ViewBag.Cartoes = _mapper.Map<List<CartaoViewModel>>(cartoes);
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

        var carrinho = _mapper.Map<Carrinho>(viewModel);
        _carrinhoService.Create(carrinho);

        return RedirectToAction(nameof(Index));
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

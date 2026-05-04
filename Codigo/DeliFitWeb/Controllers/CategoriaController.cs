using AutoMapper;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers;

public class CategoriaController : Controller
{
    private readonly ICategoriaService _categoriaService;
    private readonly IRestauranteService _restauranteService;
    private readonly IMapper _mapper;

    public CategoriaController(ICategoriaService categoriaService,
                               IRestauranteService restauranteService,
                               IMapper mapper)
    {
        _categoriaService = categoriaService;
        _restauranteService = restauranteService;
        _mapper = mapper;
    }

    // GET: Categoria — M18: listagem de todas as categorias
    public ActionResult Index()
    {
        var categoriaDto = _categoriaService.ListarCategorias();
        var viewModel = _mapper.Map<List<CategoriaViewModel>>(categoriaDto);
        return View(viewModel);
    }

    // GET: Categoria/Ver?categoria=Fitness — M19: restaurantes de uma categoria
    public ActionResult Ver(string categoria)
    {
        if (string.IsNullOrEmpty(categoria))
            return RedirectToAction(nameof(Index));

        var restaurantes = _restauranteService.GetByRestricao(categoria);
        var viewModel = _mapper.Map<List<RestauranteViewModel>>(restaurantes);

        ViewBag.Categoria = categoria;
        return View(viewModel);
    }

    // GET: Categoria/Itens?categoria=Fitness — mantido para compatibilidade
    public IActionResult Itens(string categoria)
    {
        if (string.IsNullOrEmpty(categoria))
            return RedirectToAction(nameof(Index));

        var itens = _categoriaService.ListarItensPorCategoria(categoria);
        var viewModel = _mapper.Map<List<ItemViewModel>>(itens);
        return View(viewModel);
    }

    // GET: Categoria/Buscar?termo=xxx — M23: busca de restaurantes
    public ActionResult Buscar(string? termo)
    {
        var restaurantes = _restauranteService.Buscar(termo ?? "");
        var viewModel = _mapper.Map<List<RestauranteViewModel>>(restaurantes);
        ViewBag.Termo = termo ?? "";
        return View(viewModel);
    }
}

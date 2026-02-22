using AutoMapper;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers
{
    public class CategoriaController : Controller
    {

        private readonly ICategoriaService _categoriaService;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            _categoriaService = categoriaService;
            _mapper = mapper;
        }

        // GET: CategoriaController
        public ActionResult Index()
        {
            var categoriaDto = _categoriaService.ListarCategorias();
            var viewModel = _mapper.Map<List<CategoriaViewModel>>(categoriaDto);

            return View(viewModel);
        }

        // GET: Categoria/Itens?categoria=Financeira
        public IActionResult Itens(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return RedirectToAction(nameof(Index));

            var itens = _categoriaService.ListarItensPorCategoria(categoria);
            var viewModel = _mapper.Map<List<ItemViewModel>>(itens);

            return View(viewModel);
        }
    }
}

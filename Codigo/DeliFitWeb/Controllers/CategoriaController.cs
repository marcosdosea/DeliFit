using AutoMapper;
using Core;
using Core.DTO;
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
            var viewModel = categoriaDto.Select(c => new CategoriaViewModel
            {
                Nome = c.Nome,
                QuantidadeItens = c.QuantidadeItens
            }).ToList();

            return View(viewModel);
        }

        // GET: Categoria/Itens?categoria=Financeira
        public IActionResult Itens(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return BadRequest();

            var itens =  _categoriaService.ListarItensPorCategoria(categoria);

            var viewModel = itens.Select(i => new ItemViewModel
            {
                Id = i.Id,
                Nome = i.Nome
            }).ToList();

            return View(viewModel);
        }
    }
}

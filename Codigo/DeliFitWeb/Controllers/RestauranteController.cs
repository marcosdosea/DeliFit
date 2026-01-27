using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;

        public RestauranteController(IRestauranteService restauranteService, IMapper mapper)
        {
            _restauranteService = restauranteService;
            _mapper = mapper;
        }

        // GET: RestauranteController
        public ActionResult Index()
        {
            var listaRestaurantes = _restauranteService.GetAll();
            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);

            return View(listaRestaurantesModel);
        }

        // GET: RestauranteController/Details/5
        public ActionResult Details(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            return View(restauranteModel);
        }

        // GET: RestauranteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestauranteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RestauranteViewModel restauranteModel)
        {

            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Create(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RestauranteController/Edit/5
        public ActionResult Edit(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            return View(restauranteModel);
        }

        // POST: RestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RestauranteViewModel restauranteModel)
        {
            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Edit(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RestauranteController/Delete/5
        public ActionResult Delete(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);
            
            return View(restauranteModel);
        }

        // POST: RestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, RestauranteViewModel restauranteModel)
        {
            _restauranteService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

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
            // FILTRO: Apenas restaurantes ATIVOS (Validado = true)
            var listaRestaurantes = _restauranteService.GetAll()
                .Where(r => r.Validado == true) // <-- ADICIONE ESTE FILTRO
                .ToList();

            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);

            return View(listaRestaurantesModel);
        }

        // GET: RestauranteController/ListarSolicitacoes
        public ActionResult ListarSolicitacoes()
        {
            // FILTRO: Apenas restaurantes INATIVOS/PENDENTES (Validado = false)
            var listaRestaurantes = _restauranteService.GetAll()
                .Where(r => r.Validado == false) // <-- ADICIONE ESTE FILTRO
                .ToList();

            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);

            return View(listaRestaurantesModel);
        }

        // GET: RestauranteController/Details/5
        public ActionResult Details(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            ViewBag.CanChangeStatus = false;

            return View(restauranteModel);
        }

        // GET: RestauranteController/DetailsSolicitacao/5
        public ActionResult DetailsSolicitacao(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            ViewBag.CanChangeStatus = true;

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

        // GET: RestauranteController/SolicitarAdesao
        public ActionResult SolicitarAdesao()
        {
            return View();
        }

        // POST: RestauranteController/SolicitarAdesao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolicitarAdesao(RestauranteViewModel restauranteModel)
        {

            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Create(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RestauranteController/AprovarSolicitacao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AprovarSolicitacao(uint id)
        {
            var restaurante = _restauranteService.Get(id);
            if (restaurante != null)
            {
                restaurante.Validado = true; // Ativa o restaurante
                _restauranteService.Edit(restaurante);
            }
            return RedirectToAction(nameof(ListarSolicitacoes));
        }

        // POST: RestauranteController/NegarSolicitacao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NegarSolicitacao(uint id)
        {
            _restauranteService.Delete(id);

            return RedirectToAction(nameof(ListarSolicitacoes));
        }
    }
}

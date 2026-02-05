using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers
{
    public class AtendimentoController : Controller
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IMapper _mapper;

        public AtendimentoController(IAtendimentoService atendimentoService, IMapper mapper)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
        }

        // GET: AtendimentoController
        public ActionResult Index(uint idRestaurante)
        {
            var listaAtendimentos = _atendimentoService.GetAll(idRestaurante);
            var listaAtendimentosViewModel = _mapper.Map<IEnumerable<Core.Atendimento>, IEnumerable<DeliFitWeb.Models.AtendimentoViewModel>>(listaAtendimentos);
            
            ViewBag.IdRestaurante = idRestaurante;

            return View(listaAtendimentosViewModel);
        }

        // GET: AtendimentoController/Create
        public ActionResult Create(uint? idRestaurante)
        {
            var model = new AtendimentoViewModel();
            if (idRestaurante.HasValue)
            {
                model.IdRestaurante = idRestaurante.Value;
            }
            return View(model);
        }

        // POST: AtendimentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AtendimentoViewModel atendimentoModel)
        {
            if (ModelState.IsValid)
            {
                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);
                _atendimentoService.Create(atendimento);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AtendimentoController/Edit/5
        public ActionResult Edit(uint id)
        {
            Atendimento? atendimento = _atendimentoService.Get(id);
            AtendimentoViewModel atendimentoModel = _mapper.Map<AtendimentoViewModel>(atendimento);

            return View(atendimentoModel);
        }

        // POST: AtendimentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AtendimentoViewModel atendimentoModel)
        {

            if (ModelState.IsValid)
            {
                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);
                _atendimentoService.Edit(atendimento);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AtendimentoController/Delete/5
        public ActionResult Delete(uint id)
        {
            Atendimento? atendimento = _atendimentoService.Get(id);
            AtendimentoViewModel atendimentoModel = _mapper.Map<AtendimentoViewModel>(atendimento);
            
            return View(atendimentoModel);
        }

        // POST: AtendimentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, AtendimentoViewModel atendimentoModel)
        {
            _atendimentoService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DeliFitWeb.Controllers
{
    public class EnderecoController : Controller
    {
        private readonly IEnderecoService _enderecoService;
        private readonly IMapper _mapper;

        public EnderecoController(IEnderecoService enderecoService, IMapper mapper)
        {
            _enderecoService = enderecoService;
            _mapper = mapper;
        }

        // GET: EnderecoController
        public ActionResult Index(uint idCliente)
        {
            var listaEnderecos = _enderecoService.GetAll().Where(e => e.IdCliente == idCliente);
            var listaEnderecosViewModel = _mapper.Map<List<EnderecoViewModel>>(listaEnderecos);

            ViewBag.IdCliente = idCliente;

            return View(listaEnderecosViewModel);
        }

        // GET: EnderecoController/Details/5
        public ActionResult Details(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoViewModel enderecoModel = _mapper.Map<EnderecoViewModel>(endereco);
            return View(enderecoModel);
        }

        // GET: EnderecoController/Create
        public ActionResult Create(uint? idCliente)
        {
            var model = new EnderecoViewModel();
            if (idCliente.HasValue)
            {
                model.IdCliente = idCliente.Value; // recebe pela rota/query
            }
            return View(model);
        }

        // POST: EnderecoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnderecoViewModel enderecoModel)
        {
            if (ModelState.IsValid)
            {
                var endereco = _mapper.Map<Endereco>(enderecoModel);
                _enderecoService.Create(endereco);

                return RedirectToAction(nameof(Index),
                    new { idCliente = enderecoModel.IdCliente });
            }

            return View(enderecoModel);
        }

        // GET: EnderecoController/Edit/5
        public ActionResult Edit(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoViewModel enderecoModel = _mapper.Map<EnderecoViewModel>(endereco);

            return View(enderecoModel);
        }

        // POST: EnderecoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, EnderecoViewModel enderecoModel)
        {
            if (ModelState.IsValid)
            {
                var endereco = _mapper.Map<Endereco>(enderecoModel);
                _enderecoService.Edit(endereco);

                return RedirectToAction(nameof(Index),
                    new { idCliente = enderecoModel.IdCliente });
            }

            return View(enderecoModel);
        }


        // GET: EnderecoController/Delete/5
        public ActionResult Delete(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoViewModel enderecoModel = _mapper.Map<EnderecoViewModel>(endereco);

            return View(enderecoModel);
        }

        // POST: EnderecoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, EnderecoViewModel enderecoModel)
        {
            _enderecoService.Delete(id);

            return RedirectToAction(nameof(Index),
                new { idCliente = enderecoModel.IdCliente });
        }

    }
}

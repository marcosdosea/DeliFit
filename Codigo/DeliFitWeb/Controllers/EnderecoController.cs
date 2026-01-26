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
        public ActionResult Index()
        {
            var listaEnderecos = _enderecoService.GetAll();
            var listaEnderecosViewModel = _mapper.Map<List<EnderecoModel>>(listaEnderecos);
            return View(listaEnderecosViewModel);
        }

        // GET: EnderecoController/Details/5
        public ActionResult Details(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoModel enderecoModel = _mapper.Map<EnderecoModel>(endereco);
            return View(enderecoModel);
        }

        // GET: EnderecoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EnderecoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnderecoModel enderecoModel)
        {
            if (ModelState.IsValid)
            {
                var endereco = _mapper.Map<Endereco>(enderecoModel);
                _enderecoService.Create(endereco);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: EnderecoController/Edit/5
        public ActionResult Edit(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoModel enderecoModel = _mapper.Map<EnderecoModel>(endereco);

            return View(enderecoModel);
        }

        // POST: EnderecoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EnderecoModel enderecoModel)
        {
            if (ModelState.IsValid)
            {
                var endereco = _mapper.Map<Endereco>(enderecoModel);
                _enderecoService.Edit(endereco);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: EnderecoController/Delete/5
        public ActionResult Delete(uint id)
        {
            Endereco? endereco = _enderecoService.Get(id);
            EnderecoModel enderecoModel = _mapper.Map<EnderecoModel>(endereco);

            return View(enderecoModel);
        }

        // POST: EnderecoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, EnderecoModel enderecoModel)
        {
            _enderecoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

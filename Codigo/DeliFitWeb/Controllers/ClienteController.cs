using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Models;
using Core;

namespace DeliFitWeb.Controllers
{
    public class ClienteController : Controller
    {

        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;

        public ClienteController(IClienteService clienteService, IMapper mapper)
        {
            _clienteService = clienteService;
            _mapper = mapper;
        }
        // GET: ClienteController
        public ActionResult Index()
        {
            var listaClientes = _clienteService.GetAll();
            var listaClientesViewModel = _mapper.Map<List<ClienteViewModel>>(listaClientes);

            return View(listaClientesViewModel);
        }

        // GET: ClienteController/Details/5
        public ActionResult Details(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteModel);
        }

        // GET: ClienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteViewModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Create(cliente);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);

            return View(clienteModel);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteViewModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Edit(cliente);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ClienteController/Delete/5
        public ActionResult Delete(uint id)
        {
            Cliente? cliente = _clienteService.Get(id);
            ClienteViewModel clienteModel = _mapper.Map<ClienteViewModel>(cliente);

            return View(clienteModel);
        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ClienteViewModel clienteModel)
        {
            _clienteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

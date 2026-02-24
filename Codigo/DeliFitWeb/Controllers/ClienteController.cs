using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Models;
using Core;
using Microsoft.AspNetCore.Identity;
using Core.Identity.Data;
using System.Linq;

namespace DeliFitWeb.Controllers
{
    public class ClienteController : Controller
    {

        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly IBrasilApiService _brasilApiService;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public ClienteController(
    IClienteService clienteService,
    IMapper mapper,
    IBrasilApiService brasilApiService,
    UserManager<UsuarioIdentity> userManager)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _brasilApiService = brasilApiService;
            _userManager = userManager;
        }

        // GET: ClienteController
        public ActionResult Index()
        {
            var listaClientes = _clienteService.GetAll();
            var listaClientesViewModel = _mapper.Map<List<ClienteViewModel>>(listaClientes);
            return View(listaClientesViewModel);
        }

        // GET: ClienteController/Perfil
        // Redireciona o cliente logado para o seu próprio Details
        public async Task<ActionResult> Perfil()
        {
            var userEmail = _userManager.GetUserName(User);
            var cliente = _clienteService.GetByEmail(userEmail);

            if (cliente == null)
                return NotFound("Perfil de cliente não encontrado para o usuário logado.");

            return RedirectToAction(nameof(Details), new { id = cliente.Id });
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
        public async Task<ActionResult> CreateAsync(ClienteViewModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                // Validação e normalização do telefone antes de extrair o DDD
                if (clienteModel == null || string.IsNullOrWhiteSpace(clienteModel.Telefone))
                {
                    ModelState.AddModelError("Telefone", "Telefone inválido.");
                    return View(clienteModel);
                }

                var normalized = new string(clienteModel.Telefone.Where(char.IsDigit).ToArray());
                if (normalized.Length < 2)
                {
                    ModelState.AddModelError("Telefone", "Telefone inválido.");
                    return View(clienteModel);
                }

                string ddd = normalized.Substring(0, 2);

                bool dddValido = await _brasilApiService.IsDddValidAsync(ddd);

                if (!dddValido)
                {
                    ModelState.AddModelError("Telefone", "O DDD informado é inválido.");
                    return View(clienteModel);
                }

                // Atualiza o modelo com o telefone normalizado (somente dígitos)
                clienteModel.Telefone = normalized;

                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Create(cliente);

                return RedirectToAction(nameof(Index));
            }

            return View(clienteModel);
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
                // Validação e normalização do telefone antes de salvar
                if (clienteModel == null || string.IsNullOrWhiteSpace(clienteModel.Telefone))
                {
                    ModelState.AddModelError("Telefone", "Telefone inválido.");
                    return View(clienteModel);
                }

                var normalized = new string(clienteModel.Telefone.Where(char.IsDigit).ToArray());
                if (normalized.Length != 11)
                {
                    ModelState.AddModelError("Telefone", "O telefone deve conter 11 dígitos.");
                    return View(clienteModel);
                }

                // Atualiza o modelo com o telefone normalizado
                clienteModel.Telefone = normalized;

                var cliente = _mapper.Map<Cliente>(clienteModel);
                _clienteService.Edit(cliente);
                return RedirectToAction(nameof(Index));
            }

            return View(clienteModel);
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
        public ActionResult Delete(ClienteViewModel clienteModel)
        {
            _clienteService.Delete(clienteModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
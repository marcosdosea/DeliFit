using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using DeliFitWeb.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DeliFitWeb.Controllers
{
    public class EnderecoController : Controller
    {
        private readonly IEnderecoService _enderecoService;
        private readonly IMapper _mapper;
        private readonly IClienteService _clienteService;

        public EnderecoController(IEnderecoService enderecoService, IMapper mapper, IClienteService clienteService)
        {
            _enderecoService = enderecoService;
            _mapper = mapper;
            _clienteService = clienteService;
        }

        // GET: EnderecoController
        [Authorize(Roles = "Cliente,Admin")]
        public ActionResult Index(uint? idCliente)
        {
            uint clienteIdFiltro;

            // Se foi passado um ID, usa ele (Admin vendo endereços de qualquer cliente)
            if (idCliente.HasValue)
            {
                clienteIdFiltro = idCliente.Value;
            }
            else if (User.IsInRole("Cliente"))
            {
                // Se não foi passado ID, busca da sessão (cliente vendo seus próprios endereços)
                var clienteIdSessao = GetClienteIdLogado();
                if (!clienteIdSessao.HasValue)
                {
                    TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
                    return RedirectToAction("Index", "Home");
                }
                clienteIdFiltro = clienteIdSessao.Value;
            }
            else
            {
                return BadRequest("ID do cliente não fornecido.");
            }

            var listaEnderecos = _enderecoService.GetAll().Where(e => e.IdCliente == clienteIdFiltro);
            var listaEnderecosViewModel = _mapper.Map<List<EnderecoViewModel>>(listaEnderecos);

            ViewBag.IdCliente = clienteIdFiltro;

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
        [Authorize(Roles = "Cliente")]
        public ActionResult Create(uint? idCliente)
        {
            var model = new EnderecoViewModel();

            if (idCliente.HasValue)
            {
                model.IdCliente = idCliente.Value;
            }
            else
            {
                // Busca da sessão (cliente criando seu próprio endereço)
                var clienteIdSessao = GetClienteIdLogado();
                if (clienteIdSessao.HasValue)
                {
                    model.IdCliente = clienteIdSessao.Value;
                }
                else
                {
                    TempData["Error"] = "Não foi possível identificar o cliente. Faça login novamente.";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

        // POST: EnderecoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public ActionResult Create(EnderecoViewModel enderecoModel)
        {
            // Se o IdCliente não foi enviado, tenta buscar da sessão
            if (enderecoModel.IdCliente == 0)
            {
                var clienteId = GetClienteIdLogado();
                if (clienteId.HasValue)
                {
                    enderecoModel.IdCliente = clienteId.Value;
                }
                else
                {
                    ModelState.AddModelError("", "Não foi possível identificar o cliente. Faça login novamente.");
                    return View(enderecoModel);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var endereco = _mapper.Map<Endereco>(enderecoModel);
                    _enderecoService.Create(endereco);
                    TempData["Success"] = "Endereço criado com sucesso!";

                    return RedirectToAction(nameof(Index),
                        new { idCliente = enderecoModel.IdCliente });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar endereço: {ex.Message}");
                }
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

        // Método auxiliar para obter o ID do cliente logado
        private uint? GetClienteIdLogado()
        {
            // Tenta buscar da sessão
            var clienteId = HttpContext.Session.GetClienteId();

            if (!clienteId.HasValue)
            {
                // Se não estiver na sessão, busca pelo email
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var cliente = _clienteService.GetByEmail(userEmail);

                    if (cliente != null)
                    {
                        // Armazena na sessão para próximas requisições
                        HttpContext.Session.SetClienteId(cliente.Id);
                        clienteId = cliente.Id;
                    }
                }
            }

            return clienteId;
        }
    }
}

using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using DeliFitWeb.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DeliFitWeb.Controllers;

public class CartaoController : Controller
{
    private readonly ICartaoService _cartaoService;
    private readonly IMapper _mapper;
    private readonly IClienteService _clienteService;

    public CartaoController(ICartaoService cartaoService, IMapper mapper, IClienteService clienteService)
    {
        _cartaoService = cartaoService;
        _mapper = mapper;
        _clienteService = clienteService;
    }

    [Authorize(Roles = "Cliente,Admin")]
    public ActionResult Index(uint? idCliente)
    {
        uint clienteIdFiltro;

        if (idCliente.HasValue)
        {
            clienteIdFiltro = idCliente.Value;
        }
        else if (User.IsInRole("Cliente"))
        {
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

        var listaCartoes = _cartaoService.GetByCliente(clienteIdFiltro);
        var listaViewModel = _mapper.Map<List<CartaoViewModel>>(listaCartoes);
        ViewBag.IdCliente = clienteIdFiltro;
        return View(listaViewModel);
    }

    public ActionResult Details(uint id)
    {
        var cartao = _cartaoService.Get(id);
        if (cartao == null)
        {
            TempData["Error"] = "Cartão não encontrado.";
            return RedirectToAction(nameof(Index));
        }
        var viewModel = _mapper.Map<CartaoViewModel>(cartao);
        return View(viewModel);
    }

    [Authorize(Roles = "Cliente")]
    public ActionResult Create(uint? idCliente)
    {
        var model = new CartaoViewModel();

        if (idCliente.HasValue)
        {
            model.IdCliente = idCliente.Value;
        }
        else
        {
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Cliente")]
    public ActionResult Create(CartaoViewModel viewModel)
    {
        if (viewModel.IdCliente == 0)
        {
            var clienteId = GetClienteIdLogado();
            if (clienteId.HasValue)
            {
                viewModel.IdCliente = clienteId.Value;
            }
            else
            {
                ModelState.AddModelError("", "Não foi possível identificar o cliente.");
                return View(viewModel);
            }
        }

        // Monta o DateTime de validade a partir dos campos auxiliares de mês e ano
        if (viewModel.ValidadeMes >= 1 && viewModel.ValidadeMes <= 12 && viewModel.ValidadeAno >= 2024)
        {
            viewModel.Validade = new DateTime((int)viewModel.ValidadeAno, (int)viewModel.ValidadeMes, 1);
            ModelState.Remove(nameof(viewModel.Validade));
        }
        else
        {
            ModelState.AddModelError("ValidadeMes", "Informe um mês e ano de validade válidos.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var cartao = _mapper.Map<Cartao>(viewModel);
                _cartaoService.Create(cartao);
                TempData["Success"] = "Cartão adicionado com sucesso!";
                return RedirectToAction(nameof(Index), new { idCliente = viewModel.IdCliente });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao adicionar cartão: {ex.Message}");
            }
        }

        return View(viewModel);
    }

    public ActionResult Delete(uint id)
    {
        var cartao = _cartaoService.Get(id);
        if (cartao == null)
        {
            TempData["Error"] = "Cartão não encontrado.";
            return RedirectToAction(nameof(Index));
        }
        var viewModel = _mapper.Map<CartaoViewModel>(cartao);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(uint id, CartaoViewModel viewModel)
    {
        try
        {
            _cartaoService.Delete(viewModel.Id);
            TempData["Success"] = "Cartão excluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erro ao excluir cartão: {ex.Message}";
        }
        return RedirectToAction(nameof(Index), new { idCliente = viewModel.IdCliente });
    }

    // Método auxiliar para obter o ID do cliente logado
    private uint? GetClienteIdLogado()
    {
        var clienteId = HttpContext.Session.GetClienteId();

        if (!clienteId.HasValue)
        {
            var userEmail = User.Identity?.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var cliente = _clienteService.GetByEmail(userEmail);

                if (cliente != null)
                {
                    HttpContext.Session.SetClienteId(cliente.Id);
                    clienteId = cliente.Id;
                }
            }
        }

        return clienteId;
    }
}
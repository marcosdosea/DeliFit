using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Helpers;
using DeliFitWeb.Models;
using DeliFitWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers;

public class CartaoController : Controller
{
    private readonly ICartaoService _cartaoService;
    private readonly IMapper _mapper;
    private readonly IClienteService _clienteService;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IConfiguration _configuration;

    public CartaoController(
        ICartaoService cartaoService,
        IMapper mapper,
        IClienteService clienteService,
        IMercadoPagoService mercadoPagoService,
        IConfiguration configuration)
    {
        _cartaoService = cartaoService;
        _mapper = mapper;
        _clienteService = clienteService;
        _mercadoPagoService = mercadoPagoService;
        _configuration = configuration;
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

        ViewBag.MercadoPagoPublicKey = _configuration["MercadoPago:PublicKey"];
        return View(model);
    }

    /// <summary>
    /// POST: Salva um cartão tokenizado pelos Secure Fields do Mercado Pago. Número, validade e CVV
    /// completos nunca chegam a este servidor — só o token e os dados de exibição/cobrança.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult> Create(CartaoViewModel viewModel)
    {
        if (viewModel.IdCliente == 0)
        {
            var clienteId = GetClienteIdLogado();
            if (clienteId.HasValue)
                viewModel.IdCliente = clienteId.Value;
        }

        ModelState.Remove(nameof(viewModel.Bandeira));
        ModelState.Remove(nameof(viewModel.UltimosQuatroDigitos));
        ModelState.Remove(nameof(viewModel.Validade));

        if (string.IsNullOrWhiteSpace(viewModel.Token))
            ModelState.AddModelError("", "Não foi possível ler os dados do cartão. Confira os campos e tente novamente.");

        if (!ModelState.IsValid)
        {
            ViewBag.MercadoPagoPublicKey = _configuration["MercadoPago:PublicKey"];
            return View(viewModel);
        }

        var cliente = _clienteService.Get(viewModel.IdCliente);
        if (cliente == null)
        {
            TempData["Error"] = "Cliente não encontrado.";
            return RedirectToAction("Index", "Home");
        }

        string customerId;
        try
        {
            customerId = await _mercadoPagoService.ObterOuCriarCustomerIdAsync(cliente);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erro ao registrar cliente no Mercado Pago: {ex.Message}";
            return RedirectToAction(nameof(Create), new { idCliente = viewModel.IdCliente });
        }

        var resultado = await _mercadoPagoService.SalvarCartaoAsync(customerId, viewModel.Token!);
        if (!resultado.Sucesso)
        {
            TempData["Error"] = resultado.MensagemErro ?? "Não foi possível salvar o cartão.";
            return RedirectToAction(nameof(Create), new { idCliente = viewModel.IdCliente });
        }

        var cartao = new Cartao
        {
            Nome = viewModel.Nome,
            Cpf = viewModel.Cpf,
            IdCliente = viewModel.IdCliente,
            MercadoPagoCardId = resultado.MercadoPagoCardId!,
            MercadoPagoPaymentMethodId = resultado.PaymentMethodId ?? "",
            MercadoPagoIssuerId = resultado.IssuerId,
            Bandeira = resultado.Bandeira ?? "Cartão",
            UltimosQuatroDigitos = resultado.UltimosQuatroDigitos ?? "0000",
            Validade = resultado.ExpirationMonth.HasValue && resultado.ExpirationYear.HasValue
                ? new DateTime(resultado.ExpirationYear.Value, resultado.ExpirationMonth.Value, 1)
                : DateTime.Today
        };

        try
        {
            _cartaoService.Create(cartao);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Cartão salvo no Mercado Pago, mas houve erro ao registrar localmente: {ex.Message}";
            return RedirectToAction(nameof(Create), new { idCliente = viewModel.IdCliente });
        }

        TempData["Success"] = "Cartão adicionado com sucesso!";
        return RedirectToAction(nameof(Index), new { idCliente = viewModel.IdCliente });
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
    public async Task<ActionResult> Delete(uint id, CartaoViewModel viewModel)
    {
        var cartao = _cartaoService.Get(id);

        try
        {
            if (cartao != null)
            {
                var cliente = _clienteService.Get(cartao.IdCliente);
                if (cliente?.MercadoPagoCustomerId != null)
                    await _mercadoPagoService.RemoverCartaoAsync(cliente.MercadoPagoCustomerId, cartao.MercadoPagoCardId);
            }

            _cartaoService.Delete(id);
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

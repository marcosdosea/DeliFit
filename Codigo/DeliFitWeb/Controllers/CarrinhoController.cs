using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Helpers;
using DeliFitWeb.Services;

namespace DeliFitWeb.Controllers;

[Authorize(Roles = "Cliente")]
public class CarrinhoController : Controller
{
    private readonly ICarrinhoService _carrinhoService;
    private readonly IClienteService _clienteService;
    private readonly ICartaoService _cartaoService;
    private readonly IItemService _itemService;
    private readonly IPedidoService _pedidoService;
    private readonly IEnderecoService _enderecoService;
    private readonly IRestauranteService _restauranteService;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    private const string SessaoCarrinho = "CarrinhoSessao";
    private const string SessaoFormaPagamento = "FormaPagamento";
    private const string SessaoIdCartao = "CarrinhoIdCartao";
    private const string SessaoIdEndereco = "CarrinhoIdEndereco";
    private const string SessaoMercadoPagoPaymentId = "MercadoPagoPaymentId";
    private const string SessaoMercadoPagoStatus = "MercadoPagoStatus";

    public CarrinhoController(
        ICarrinhoService carrinhoService,
        IClienteService clienteService,
        ICartaoService cartaoService,
        IItemService itemService,
        IPedidoService pedidoService,
        IEnderecoService enderecoService,
        IRestauranteService restauranteService,
        IMercadoPagoService mercadoPagoService,
        IConfiguration configuration,
        IMapper mapper)
    {
        _carrinhoService = carrinhoService;
        _clienteService = clienteService;
        _cartaoService = cartaoService;
        _itemService = itemService;
        _pedidoService = pedidoService;
        _enderecoService = enderecoService;
        _restauranteService = restauranteService;
        _mercadoPagoService = mercadoPagoService;
        _configuration = configuration;
        _mapper = mapper;
    }

    // ─── M14: Tela do carrinho ────────────────────────────────────────────────

    /// <summary>
    /// GET: Exibe o carrinho atual (tela M14)
    /// </summary>
    public ActionResult Index()
    {
        var itens = ObterItensDaSessao();
        var clienteId = GetClienteIdLogado();

        // Endereços do cliente para seleção
        if (clienteId.HasValue)
        {
            var enderecos = _enderecoService.GetAll()
                .Where(e => e.IdCliente == clienteId.Value)
                .ToList();
            ViewBag.Enderecos = enderecos;

            var idEnderecoSelecionado = HttpContext.Session.GetInt32(SessaoIdEndereco);
            ViewBag.IdEnderecoSelecionado = idEnderecoSelecionado;
        }

        var formaPagamento = HttpContext.Session.GetString(SessaoFormaPagamento);
        ViewBag.FormaPagamento = formaPagamento;

        var idCartaoSessao = HttpContext.Session.GetInt32(SessaoIdCartao);
        ViewBag.IdCartaoSelecionado = idCartaoSessao;

        // Nome do cartão selecionado (para exibição)
        if (idCartaoSessao.HasValue)
        {
            var cartao = _cartaoService.Get((uint)idCartaoSessao.Value);
            ViewBag.CartaoSelecionado = cartao;
        }

        return View(itens);
    }

    // ─── Adicionar item (vindo do modal M09) ─────────────────────────────────

    /// <summary>
    /// POST: Adiciona item ao carrinho de sessão. Chamado pelo modal "Montar Refeição".
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AdicionarItem(uint idItem, int quantidade, string? tamanho, string? observacao)
    {
        var item = _itemService.Get(idItem);
        if (item == null)
        {
            TempData["Error"] = "Item não encontrado.";
            return RedirectToAction("Index");
        }

        var restaurante = _restauranteService.Get(item.IdRestaurante);

        var itens = ObterItensDaSessao();

        // Verifica se o carrinho já tem itens de outro restaurante
        if (itens.Any() && itens.First().IdRestaurante != item.IdRestaurante)
        {
            TempData["Error"] = "Seu carrinho já tem itens de outro restaurante. Finalize ou esvazie o carrinho antes de adicionar itens de um novo restaurante.";
            return RedirectToAction("VerEstabelecimento", "Restaurante", new { id = item.IdRestaurante });
        }

        // Verifica se já existe o mesmo item+tamanho no carrinho
        var existente = itens.FirstOrDefault(i => i.IdItem == idItem && i.Tamanho == tamanho);
        if (existente != null)
        {
            existente.Quantidade += quantidade;
        }
        else
        {
            itens.Add(new CarrinhoSessaoItem
            {
                IdItem = idItem,
                NomeItem = item.Nome,
                PrecoUnitario = item.Preco,
                Quantidade = quantidade,
                Tamanho = tamanho,
                Observacao = observacao,
                IdRestaurante = item.IdRestaurante,
                NomeRestaurante = restaurante?.NomeRestaurante ?? ""
            });
        }

        SalvarItensSessao(itens);
        TempData["Success"] = $"'{item.Nome}' adicionado ao carrinho!";
        return RedirectToAction("VerEstabelecimento", "Restaurante", new { id = item.IdRestaurante });
    }

    // ─── Remover item ─────────────────────────────────────────────────────────

    /// <summary>
    /// POST: Remove um item do carrinho pelo índice
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RemoverItem(int indice)
    {
        var itens = ObterItensDaSessao();
        if (indice >= 0 && indice < itens.Count)
            itens.RemoveAt(indice);
        SalvarItensSessao(itens);
        return RedirectToAction(nameof(Index));
    }

    // ─── Atualizar quantidade ─────────────────────────────────────────────────

    /// <summary>
    /// POST: Atualiza a quantidade de um item do carrinho
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AtualizarQuantidade(int indice, int quantidade)
    {
        var itens = ObterItensDaSessao();
        if (indice >= 0 && indice < itens.Count)
        {
            if (quantidade <= 0)
                itens.RemoveAt(indice);
            else
                itens[indice].Quantidade = quantidade;
        }
        SalvarItensSessao(itens);
        return RedirectToAction(nameof(Index));
    }

    // ─── Esvaziar carrinho ────────────────────────────────────────────────────

    /// <summary>
    /// POST: Remove todos os itens do carrinho de sessão
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Esvaziar()
    {
        HttpContext.Session.Remove(SessaoCarrinho);
        HttpContext.Session.Remove(SessaoFormaPagamento);
        HttpContext.Session.Remove(SessaoIdCartao);
        HttpContext.Session.Remove(SessaoIdEndereco);
        HttpContext.Session.Remove(SessaoMercadoPagoPaymentId);
        HttpContext.Session.Remove(SessaoMercadoPagoStatus);
        return RedirectToAction(nameof(Index));
    }

    // ─── M15: Selecionar forma de pagamento ───────────────────────────────────

    /// <summary>
    /// GET: Tela M15 - Selecionar forma de pagamento
    /// </summary>
    public ActionResult SelecionarPagamento()
    {
        var clienteId = GetClienteIdLogado();
        if (!clienteId.HasValue)
        {
            TempData["Error"] = "Faça login para continuar.";
            return RedirectToAction("Index", "Home");
        }

        var cartoes = _cartaoService.GetByCliente(clienteId.Value).ToList();
        ViewBag.Cartoes = cartoes;

        var formaSelecionada = HttpContext.Session.GetString(SessaoFormaPagamento);
        var idCartaoSelecionado = HttpContext.Session.GetInt32(SessaoIdCartao);

        ViewBag.FormaSelecionada = formaSelecionada;
        ViewBag.IdCartaoSelecionado = idCartaoSelecionado;

        ViewBag.MercadoPagoPublicKey = _configuration["MercadoPago:PublicKey"];
        ViewBag.ValorTotal = ObterItensDaSessao().Sum(i => i.Subtotal);
        ViewBag.PagamentoCartaoStatus = HttpContext.Session.GetString(SessaoMercadoPagoStatus);

        return View();
    }

    // ─── Processar pagamento com cartão via Mercado Pago ──────────────────────

    /// <summary>
    /// POST: Chamado via AJAX pelo Card Payment Brick após o tokenizador gerar o cardToken.
    /// Cobra o valor total do carrinho no Mercado Pago e guarda o resultado na sessão.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> ProcessarPagamentoCartao(
        string token,
        string paymentMethodId,
        string? issuerId,
        int installments)
    {
        var clienteId = GetClienteIdLogado();
        if (!clienteId.HasValue)
            return Json(new { sucesso = false, mensagem = "Faça login para continuar." });

        var cliente = _clienteService.Get(clienteId.Value);
        if (cliente == null)
            return Json(new { sucesso = false, mensagem = "Cliente não encontrado." });

        var valorTotal = ObterItensDaSessao().Sum(i => i.Subtotal);
        if (valorTotal <= 0)
        {
            return Json(new { sucesso = false, mensagem = "Carrinho vazio." });
        }

        // Uma string vazia (em vez de ausente) faz o Mercado Pago tentar buscar um emissor
        // inexistente e recusar com "not_result_by_params" — trata como "sem emissor".
        if (string.IsNullOrWhiteSpace(issuerId))
            issuerId = null;

        var resultado = await _mercadoPagoService.ProcessarPagamentoCartaoAsync(
            token,
            valorTotal,
            paymentMethodId,
            installments,
            issuerId,
            cliente.Email,
            cliente.Cpf,
            "Pedido DeliFit",
            cliente.MercadoPagoCustomerId);

        HttpContext.Session.SetString(SessaoMercadoPagoStatus, resultado.Status);
        if (!string.IsNullOrEmpty(resultado.MercadoPagoPaymentId))
            HttpContext.Session.SetString(SessaoMercadoPagoPaymentId, resultado.MercadoPagoPaymentId);

        return Json(new
        {
            sucesso = resultado.Sucesso,
            status = resultado.Status,
            mensagem = resultado.Sucesso
                ? "Pagamento aprovado!"
                : (resultado.MensagemErro ?? "Pagamento não aprovado. Tente outro cartão.")
        });
    }

    /// <summary>
    /// POST: Salva a forma de pagamento selecionada na sessão e volta ao carrinho
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SelecionarPagamento(string formaPagamento, uint? idCartao)
    {
        if (string.IsNullOrEmpty(formaPagamento))
        {
            TempData["Error"] = "Selecione uma forma de pagamento.";
            return RedirectToAction(nameof(SelecionarPagamento));
        }

        // Validação: cartão exige id
        if (formaPagamento == "C" && (!idCartao.HasValue || idCartao == 0))
        {
            TempData["Error"] = "Selecione um cartão para pagamento com cartão.";
            return RedirectToAction(nameof(SelecionarPagamento));
        }

        HttpContext.Session.SetString(SessaoFormaPagamento, formaPagamento);

        if (formaPagamento == "C" && idCartao.HasValue)
            HttpContext.Session.SetInt32(SessaoIdCartao, (int)idCartao.Value);
        else
            HttpContext.Session.Remove(SessaoIdCartao);

        TempData["Success"] = "Forma de pagamento selecionada!";
        return RedirectToAction(nameof(Index));
    }

    // ─── Selecionar endereço ──────────────────────────────────────────────────

    /// <summary>
    /// POST: Salva o endereço de entrega selecionado na sessão
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SelecionarEndereco(uint idEndereco)
    {
        HttpContext.Session.SetInt32(SessaoIdEndereco, (int)idEndereco);
        return RedirectToAction(nameof(Index));
    }

    // ─── Fazer Pedido ─────────────────────────────────────────────────────────

    /// <summary>
    /// POST: Cria o Carrinho + Pedido(s) + PedidoItems no banco. Fluxo final.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult FazerPedido()
    {
        var clienteId = GetClienteIdLogado();
        if (!clienteId.HasValue)
        {
            TempData["Error"] = "Faça login para continuar.";
            return RedirectToAction("Index", "Home");
        }

        var itens = ObterItensDaSessao();
        if (!itens.Any())
        {
            TempData["Error"] = "Seu carrinho está vazio.";
            return RedirectToAction(nameof(Index));
        }

        var formaPagamento = HttpContext.Session.GetString(SessaoFormaPagamento);
        if (string.IsNullOrEmpty(formaPagamento))
        {
            TempData["Error"] = "Selecione uma forma de pagamento antes de finalizar.";
            return RedirectToAction(nameof(Index));
        }

        var idEnderecoSessao = HttpContext.Session.GetInt32(SessaoIdEndereco);
        if (!idEnderecoSessao.HasValue)
        {
            TempData["Error"] = "Selecione um endereço de entrega antes de finalizar.";
            return RedirectToAction(nameof(Index));
        }

        uint? idCartao = null;
        string? mercadoPagoPaymentId = null;
        string? mercadoPagoStatus = null;
        if (formaPagamento == "C")
        {
            var idCartaoSessao = HttpContext.Session.GetInt32(SessaoIdCartao);
            if (!idCartaoSessao.HasValue)
            {
                TempData["Error"] = "Selecione um cartão para pagamento com cartão.";
                return RedirectToAction(nameof(Index));
            }
            idCartao = (uint)idCartaoSessao.Value;

            mercadoPagoStatus = HttpContext.Session.GetString(SessaoMercadoPagoStatus);
            mercadoPagoPaymentId = HttpContext.Session.GetString(SessaoMercadoPagoPaymentId);
            if (mercadoPagoStatus != "approved")
            {
                TempData["Error"] = "Conclua o pagamento com cartão antes de finalizar o pedido.";
                return RedirectToAction(nameof(SelecionarPagamento));
            }
        }

        try
        {
            var restaurantesNoCarrinho = itens.Select(i => i.IdRestaurante).Distinct().ToList();

            // Tenta criar o Carrinho no banco — captura falha isoladamente
            uint idCarrinho = 0;
            try
            {
                var carrinhoEntidade = new Carrinho
                {
                    IdCliente = clienteId.Value,
                    FormaDePagamento = formaPagamento,
                    ValorFrete = 0,
                    IdCartao = idCartao,
                    Observacao = null,
                    MercadoPagoPaymentId = mercadoPagoPaymentId,
                    StatusPagamentoCartao = mercadoPagoStatus
                };
                idCarrinho = _carrinhoService.Create(carrinhoEntidade);
            }
            catch
            {
                // Se Carrinho não puder ser criado (campos ausentes no modelo),
                // busca o último carrinho existente do cliente
                var carrinhoExistente = _carrinhoService.GetAll()
                    .Where(c => c.IdCliente == clienteId.Value)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (carrinhoExistente != null)
                    idCarrinho = carrinhoExistente.Id;
                else
                    throw new Exception("Não foi possível criar ou encontrar o carrinho do cliente.");
            }

            // Para cada restaurante, cria um Pedido
            uint idPedidoCriado = 0;
            foreach (var idRestaurante in restaurantesNoCarrinho)
            {
                var itensDoPedido = itens.Where(i => i.IdRestaurante == idRestaurante).ToList();
                var totalPedido = itensDoPedido.Sum(i => i.Subtotal);

                var pedido = new Pedido
                {
                    Data = DateTime.Now,
                    Preco = totalPedido,
                    IdRestaurante = idRestaurante,
                    IdCarrinho = idCarrinho
                };

                // EF resolve IdPedido automaticamente via navigation property
                foreach (var it in itensDoPedido)
                {
                    pedido.Pedidoitems.Add(new Pedidoitem
                    {
                        IdItem = it.IdItem,
                        Quantidade = it.Quantidade,
                        Preco = it.PrecoUnitario
                    });
                }

                idPedidoCriado = _pedidoService.Create(pedido);
            }

            // Limpa sessão
            HttpContext.Session.Remove(SessaoCarrinho);
            HttpContext.Session.Remove(SessaoFormaPagamento);
            HttpContext.Session.Remove(SessaoIdCartao);
            HttpContext.Session.Remove(SessaoIdEndereco);
            HttpContext.Session.Remove(SessaoMercadoPagoPaymentId);
            HttpContext.Session.Remove(SessaoMercadoPagoStatus);

            TempData["Success"] = "Pedido realizado com sucesso! 🎉";
            return RedirectToAction("Acompanhar", "Pedido", new { id = idPedidoCriado });
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            TempData["Error"] = $"Erro ao realizar pedido: {msg}";
            return RedirectToAction(nameof(Index));
        }
    }

    // ─── Auxiliares ───────────────────────────────────────────────────────────

    private List<CarrinhoSessaoItem> ObterItensDaSessao()
    {
        return HttpContext.Session.GetObject<List<CarrinhoSessaoItem>>(SessaoCarrinho)
               ?? new List<CarrinhoSessaoItem>();
    }

    private void SalvarItensSessao(List<CarrinhoSessaoItem> itens)
    {
        HttpContext.Session.SetObject(SessaoCarrinho, itens);
    }

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

    // Expõe contagem de itens para o badge no header (usada via ViewComponent ou AJAX)
    [AllowAnonymous]
    public JsonResult ContarItens()
    {
        var itens = ObterItensDaSessao();
        return Json(new { total = itens.Sum(i => i.Quantidade) });
    }
}

using Core;
using Core.Service;
using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Customer;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Error;

namespace DeliFitWeb.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<MercadoPagoService> _logger;

    public MercadoPagoService(IConfiguration configuration, IClienteService clienteService, ILogger<MercadoPagoService> logger)
    {
        _clienteService = clienteService;
        _logger = logger;

        var accessToken = configuration["MercadoPago:AccessToken"];
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new InvalidOperationException("MercadoPago:AccessToken não configurado (verifique o arquivo .env).");

        MercadoPagoConfig.AccessToken = accessToken;
    }

    /// <summary>
    /// Monta uma mensagem detalhada a partir do erro da API: o Mercado Pago costuma colocar o
    /// motivo específico em ApiError.Cause/Errors, não na mensagem genérica de topo.
    /// </summary>
    private static string FormatApiError(MercadoPagoApiException ex)
    {
        var partes = new List<string>();

        if (!string.IsNullOrWhiteSpace(ex.ApiError?.Message))
            partes.Add(ex.ApiError.Message);

        var causas = (ex.ApiError?.Cause ?? Enumerable.Empty<ApiErrorCause>())
            .Concat(ex.ApiError?.Errors ?? Enumerable.Empty<ApiErrorCause>());

        foreach (var causa in causas)
        {
            var texto = causa.Description ?? causa.Message;
            if (!string.IsNullOrWhiteSpace(texto))
                partes.Add(causa.Code != null ? $"{causa.Code}: {texto}" : texto);
            else if (!string.IsNullOrWhiteSpace(causa.Code))
                partes.Add(causa.Code);

            if (causa.Details is { Count: > 0 })
                partes.Add(string.Join(", ", causa.Details));
        }

        if (partes.Count == 0)
            partes.Add(ex.Message);

        return string.Join(" | ", partes.Distinct());
    }

    public async Task<PagamentoCartaoResultado> ProcessarPagamentoCartaoAsync(
        string cardToken,
        decimal valor,
        string paymentMethodId,
        int parcelas,
        string? issuerId,
        string emailPagador,
        string cpfPagador,
        string descricao,
        string? customerId = null)
    {
        // Sem payment_method_id o Mercado Pago não consegue resolver o pagamento e recusa com
        // "not_result_by_params" — melhor falhar aqui com uma mensagem clara do que mandar vazio.
        if (string.IsNullOrWhiteSpace(paymentMethodId))
        {
            return new PagamentoCartaoResultado
            {
                Sucesso = false,
                Status = "rejected",
                MensagemErro = "Não foi possível identificar a bandeira deste cartão salvo. Exclua-o e cadastre-o novamente."
            };
        }

        var request = new PaymentCreateRequest
        {
            TransactionAmount = valor,
            Token = cardToken,
            Description = descricao,
            Installments = parcelas,
            PaymentMethodId = paymentMethodId,
            IssuerId = issuerId,
            Payer = new PaymentPayerRequest
            {
                // Token gerado a partir de um cartão salvo no cofre (Customer Cards): o Mercado Pago
                // exige o payer.id do Customer dono do cartão, senão recusa com "Customer not found".
                Type = customerId != null ? "customer" : null,
                Id = customerId,
                Email = emailPagador,
                Identification = new IdentificationRequest
                {
                    Type = "CPF",
                    Number = cpfPagador
                }
            }
        };

        // Evita cobrança duplicada em caso de retry de rede.
        var requestOptions = new RequestOptions();
        requestOptions.CustomHeaders.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var client = new PaymentClient();

        try
        {
            var payment = await client.CreateAsync(request, requestOptions);

            return new PagamentoCartaoResultado
            {
                Sucesso = payment.Status == "approved",
                MercadoPagoPaymentId = payment.Id?.ToString(),
                Status = payment.Status ?? "unknown",
                StatusDetail = payment.StatusDetail
            };
        }
        catch (MercadoPagoApiException ex)
        {
            var detalhe = FormatApiError(ex);
            _logger.LogWarning(ex, "Pagamento Mercado Pago recusado pela API. Cause: {Cause}", detalhe);

            return new PagamentoCartaoResultado
            {
                Sucesso = false,
                Status = "rejected",
                MensagemErro = detalhe
            };
        }
    }

    public async Task<string> ObterOuCriarCustomerIdAsync(Cliente cliente)
    {
        if (!string.IsNullOrEmpty(cliente.MercadoPagoCustomerId))
            return cliente.MercadoPagoCustomerId;

        var client = new CustomerClient();
        var nomes = cliente.Nome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        string customerId;
        try
        {
            var customer = await client.CreateAsync(new CustomerRequest
            {
                Email = cliente.Email,
                FirstName = nomes.ElementAtOrDefault(0) ?? cliente.Nome,
                LastName = nomes.ElementAtOrDefault(1),
                Identification = new IdentificationRequest { Type = "CPF", Number = cliente.Cpf }
            });
            customerId = customer.Id;
        }
        catch (MercadoPagoApiException createEx)
        {
            var detalheCreate = FormatApiError(createEx);
            _logger.LogWarning(createEx, "Falha ao criar Customer no Mercado Pago para {Email}: {Cause}", cliente.Email, detalheCreate);

            // E-mail já cadastrado como Customer em uma execução/teste anterior: tenta reaproveitar o existente.
            try
            {
                var busca = await client.SearchAsync(new SearchRequest
                {
                    Filters = new Dictionary<string, object> { ["email"] = cliente.Email }
                });
                var existente = busca.Results?.FirstOrDefault();
                if (existente == null)
                    throw new InvalidOperationException(
                        $"Mercado Pago recusou a criação do cliente: {detalheCreate}");
                customerId = existente.Id;
            }
            catch (MercadoPagoApiException searchEx)
            {
                throw new InvalidOperationException(
                    $"Mercado Pago recusou a criação ({detalheCreate}) e a busca ({FormatApiError(searchEx)}) do cliente.");
            }
        }

        cliente.MercadoPagoCustomerId = customerId;
        _clienteService.Edit(cliente);
        return customerId;
    }

    public async Task<CartaoSalvoResultado> SalvarCartaoAsync(string customerId, string token)
    {
        var client = new CustomerClient();

        try
        {
            var card = await client.CreateCardAsync(customerId, new CustomerCardCreateRequest { Token = token });

            // Sem payment_method_id o cartão fica salvo mas nunca poderá ser cobrado (a API de
            // pagamentos recusa com "not_result_by_params"). Trata como falha para não gerar lixo.
            if (string.IsNullOrWhiteSpace(card.PaymentMethod?.Id))
            {
                _logger.LogWarning("Cartão salvo no Mercado Pago sem payment_method_id (card {CardId}).", card.Id);
                return new CartaoSalvoResultado
                {
                    Sucesso = false,
                    MensagemErro = "O Mercado Pago não conseguiu identificar a bandeira deste cartão. Confira o número digitado e tente novamente."
                };
            }

            return new CartaoSalvoResultado
            {
                Sucesso = true,
                MercadoPagoCardId = card.Id,
                PaymentMethodId = card.PaymentMethod?.Id,
                IssuerId = card.Issuer?.Id?.ToString(),
                Bandeira = card.PaymentMethod?.Name,
                UltimosQuatroDigitos = card.LastFourDigits,
                ExpirationMonth = card.ExpirationMonth,
                ExpirationYear = card.ExpirationYear
            };
        }
        catch (MercadoPagoApiException ex)
        {
            var detalhe = FormatApiError(ex);
            _logger.LogWarning(ex, "Falha ao salvar cartão no Mercado Pago. Cause: {Cause}", detalhe);

            return new CartaoSalvoResultado
            {
                Sucesso = false,
                MensagemErro = detalhe
            };
        }
    }

    public async Task RemoverCartaoAsync(string customerId, string cardId)
    {
        var client = new CustomerClient();
        try
        {
            await client.DeleteCardAsync(customerId, cardId);
        }
        catch (MercadoPagoApiException ex)
        {
            // Cartão pode já ter sido removido do cofre; não bloqueia a exclusão local.
            _logger.LogWarning(ex, "Falha ao remover cartão {CardId} do cofre do Mercado Pago.", cardId);
        }
    }
}

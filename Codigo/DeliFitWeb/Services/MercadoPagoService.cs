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

    public async Task<PagamentoCartaoResultado> ProcessarPagamentoCartaoAsync(
        string cardToken,
        decimal valor,
        string paymentMethodId,
        int parcelas,
        string? issuerId,
        string emailPagador,
        string cpfPagador,
        string descricao)
    {
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
            _logger.LogWarning(ex, "Pagamento Mercado Pago recusado pela API. Cause: {Cause}", ex.ApiError?.Message);

            return new PagamentoCartaoResultado
            {
                Sucesso = false,
                Status = "rejected",
                MensagemErro = ex.ApiError?.Message ?? "Pagamento recusado pelo Mercado Pago."
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
            _logger.LogWarning(createEx, "Falha ao criar Customer no Mercado Pago para {Email}: {Cause}", cliente.Email, createEx.ApiError?.Message);

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
                        $"Mercado Pago recusou a criação do cliente: {createEx.ApiError?.Message ?? createEx.Message}");
                customerId = existente.Id;
            }
            catch (MercadoPagoApiException searchEx)
            {
                throw new InvalidOperationException(
                    $"Mercado Pago recusou a criação ({createEx.ApiError?.Message ?? createEx.Message}) e a busca ({searchEx.ApiError?.Message ?? searchEx.Message}) do cliente.");
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

            return new CartaoSalvoResultado
            {
                Sucesso = true,
                MercadoPagoCardId = card.Id,
                PaymentMethodId = card.PaymentMethod?.Id,
                Bandeira = card.PaymentMethod?.Name,
                UltimosQuatroDigitos = card.LastFourDigits,
                ExpirationMonth = card.ExpirationMonth,
                ExpirationYear = card.ExpirationYear
            };
        }
        catch (MercadoPagoApiException ex)
        {
            _logger.LogWarning(ex, "Falha ao salvar cartão no Mercado Pago. Cause: {Cause}", ex.ApiError?.Message);

            return new CartaoSalvoResultado
            {
                Sucesso = false,
                MensagemErro = ex.ApiError?.Message ?? "Não foi possível salvar o cartão."
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

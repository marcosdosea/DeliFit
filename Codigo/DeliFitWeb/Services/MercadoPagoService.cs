using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Http;

namespace DeliFitWeb.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly ILogger<MercadoPagoService> _logger;

    public MercadoPagoService(IConfiguration configuration, ILogger<MercadoPagoService> logger)
    {
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
}

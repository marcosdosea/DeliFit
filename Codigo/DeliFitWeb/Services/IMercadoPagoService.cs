namespace DeliFitWeb.Services;

public class PagamentoCartaoResultado
{
    public bool Sucesso { get; set; }
    public string? MercadoPagoPaymentId { get; set; }
    public string Status { get; set; } = "";
    public string? StatusDetail { get; set; }
    public string? MensagemErro { get; set; }
}

public interface IMercadoPagoService
{
    Task<PagamentoCartaoResultado> ProcessarPagamentoCartaoAsync(
        string cardToken,
        decimal valor,
        string paymentMethodId,
        int parcelas,
        string? issuerId,
        string emailPagador,
        string cpfPagador,
        string descricao);
}

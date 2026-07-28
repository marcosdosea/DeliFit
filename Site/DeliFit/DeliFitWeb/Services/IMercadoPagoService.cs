using Core;

namespace DeliFitWeb.Services;

public class PagamentoCartaoResultado
{
    public bool Sucesso { get; set; }
    public string? MercadoPagoPaymentId { get; set; }
    public string Status { get; set; } = "";
    public string? StatusDetail { get; set; }
    public string? MensagemErro { get; set; }
}

public class CartaoSalvoResultado
{
    public bool Sucesso { get; set; }
    public string? MercadoPagoCardId { get; set; }
    public string? PaymentMethodId { get; set; }
    public string? IssuerId { get; set; }
    public string? Bandeira { get; set; }
    public string? UltimosQuatroDigitos { get; set; }
    public int? ExpirationMonth { get; set; }
    public int? ExpirationYear { get; set; }
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
        string descricao,
        string? customerId = null);

    /// <summary>
    /// Retorna o Customer do Mercado Pago já associado ao cliente ou cria um novo
    /// (e persiste o id em Cliente.MercadoPagoCustomerId).
    /// </summary>
    Task<string> ObterOuCriarCustomerIdAsync(Cliente cliente);

    /// <summary>
    /// Salva um cartão tokenizado no cofre (Customer Cards) do Mercado Pago.
    /// </summary>
    Task<CartaoSalvoResultado> SalvarCartaoAsync(string customerId, string token);

    /// <summary>
    /// Remove um cartão salvo do cofre do Mercado Pago.
    /// </summary>
    Task RemoverCartaoAsync(string customerId, string cardId);
}

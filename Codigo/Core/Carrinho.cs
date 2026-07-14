namespace Core;

public partial class Carrinho
{
    public uint Id { get; set; }

    public string? Observacao { get; set; }

    public uint IdCliente { get; set; }

    /// <summary>
    /// P para PIX,C para CARTÃO, D para DINHEIRO 
    /// </summary>
    public string FormaDePagamento { get; set; } = null!;

    public decimal ValorFrete { get; set; }

    public uint? IdCartao { get; set; }

    /// <summary>
    /// Id do pagamento retornado pelo Mercado Pago (preenchido apenas quando FormaDePagamento = 'C').
    /// </summary>
    public string? MercadoPagoPaymentId { get; set; }

    /// <summary>
    /// Status do pagamento retornado pelo Mercado Pago: approved, pending, in_process, rejected, refunded, cancelled.
    /// </summary>
    public string? StatusPagamentoCartao { get; set; }

    public virtual Cartao? IdCartaoNavigation { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

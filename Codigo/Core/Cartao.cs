using System;
using System.Collections.Generic;

namespace Core;

public partial class Cartao
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    /// <summary>
    /// Id do cartão no cofre do Mercado Pago (Customer Cards API). O número e o CVV
    /// completos nunca são armazenados aqui — apenas essa referência tokenizada.
    /// </summary>
    public string MercadoPagoCardId { get; set; } = null!;

    /// <summary>
    /// Id do meio de pagamento no Mercado Pago (ex: "master", "visa"), necessário para cobrar.
    /// </summary>
    public string MercadoPagoPaymentMethodId { get; set; } = null!;

    /// <summary>
    /// Id do emissor (banco) do cartão no Mercado Pago. Alguns cartões exigem esse valor na
    /// cobrança, senão a API recusa com "No result found for the given parameters".
    /// </summary>
    public string? MercadoPagoIssuerId { get; set; }

    public string UltimosQuatroDigitos { get; set; } = null!;

    public string Bandeira { get; set; } = null!;

    public DateTime Validade { get; set; }

    public string Cpf { get; set; } = null!;

    public uint IdCliente { get; set; }

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Core;

public partial class Cartao
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public DateTime Validade { get; set; }

    public string Cpf { get; set; } = null!;

    public uint IdCliente { get; set; }

    public uint IdCarrinho { get; set; }

    public virtual Carrinho IdCarrinhoNavigation { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

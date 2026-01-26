using System;
using System.Collections.Generic;

namespace Core;

public partial class Avaliacao
{
    public uint Id { get; set; }

    public decimal Nota { get; set; }

    public string Descricao { get; set; } = null!;

    public uint IdCliente { get; set; }

    public uint IdPedido { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}

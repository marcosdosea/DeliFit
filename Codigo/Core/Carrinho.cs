using System;
using System.Collections.Generic;

namespace Core;

public partial class Carrinho
{
    public uint Id { get; set; }

    public string? Observação { get; set; }

    public uint IdCliente { get; set; }

    /// <summary>
    /// P para PIX,C para CARTÃO, D para DINHEIRO 
    /// </summary>
    public string FormaDePagamento { get; set; } = null!;

    public virtual ICollection<Cartao> Cartaos { get; set; } = new List<Cartao>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

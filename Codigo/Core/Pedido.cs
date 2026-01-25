using System;
using System.Collections.Generic;

namespace Core;

public partial class Pedido
{
    public uint Id { get; set; }

    public DateTime Data { get; set; }

    public decimal Preco { get; set; }

    public uint IdRestaurante { get; set; }

    public uint IdCarrinho { get; set; }

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual Carrinho IdCarrinhoNavigation { get; set; } = null!;

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

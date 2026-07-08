using System;
using System.Collections.Generic;

namespace Core;

public partial class Item
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public float Calorias { get; set; }

    public float? Carboidratos { get; set; }

    public float? Gordura { get; set; }

    public float? Proteina { get; set; }

    public string? Restricao { get; set; }

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

    public string? Tamanho { get; set; }

    public string? Volume { get; set; }

    public uint IdRestaurante { get; set; }

    public byte[]? Foto { get; set; }

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;

    public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    public virtual ICollection<Pedidoitem> Pedidoitems { get; set; } = new List<Pedidoitem>();
}
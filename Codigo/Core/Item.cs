namespace Core;

public partial class Item
{
    public uint Id { get; set; }

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

    public uint IdConsumoCalorico { get; set; }

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;

    public virtual ICollection<Pedidoitem> Pedidoitems { get; set; } = new List<Pedidoitem>();
}

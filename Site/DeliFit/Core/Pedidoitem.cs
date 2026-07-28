namespace Core;

public partial class Pedidoitem
{
    public uint IdPedido { get; set; }

    public uint IdItem { get; set; }

    public int Quantidade { get; set; }

    public decimal Preco { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}

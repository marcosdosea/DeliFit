namespace Core;

public partial class Pedido
{
    public uint Id { get; set; }

    public DateTime? Data { get; set; }

    public decimal Preco { get; set; }

    public uint IdRestaurante { get; set; }

    public uint IdCarrinho { get; set; }

    public char? Status { get; set; } = 'P'; // P=Pendente, E=EmPreparo, S=EmEntrega, F=Finalizado

    public enum StatusPedido
    {
        Pendente,       // P
        EmPreparo,      // E
        EmEntrega,      // S
        Finalizado      // F
    }

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual Carrinho IdCarrinhoNavigation { get; set; } = null!;

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;

    public virtual ICollection<Pedidoitem> Pedidoitems { get; set; } = new List<Pedidoitem>();
}

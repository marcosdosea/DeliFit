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

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

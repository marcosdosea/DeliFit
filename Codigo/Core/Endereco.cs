namespace Core;

public partial class Endereco
{
    public uint Id { get; set; }

    public string Rua { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public uint IdCliente { get; set; }

    public string Label { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

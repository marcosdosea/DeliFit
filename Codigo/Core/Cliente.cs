namespace Core;

public partial class Cliente
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Id do "Customer" do Mercado Pago associado a este cliente (cofre de cartões salvos).
    /// Criado sob demanda no primeiro cartão salvo.
    /// </summary>
    public string? MercadoPagoCustomerId { get; set; }

    /// <summary>
    /// Indica se o cliente está ativo. Clientes com carrinhos/pedidos vinculados não podem
    /// ser removidos do banco (violaria integridade referencial), então "excluir" desativa
    /// o cliente em vez de apagar o registro, preservando o histórico de pedidos.
    /// </summary>
    public bool Ativo { get; set; } = true;

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual ICollection<Cartao> Cartaos { get; set; } = new List<Cartao>();

    public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
}

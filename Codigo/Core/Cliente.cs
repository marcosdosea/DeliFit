using System;
using System.Collections.Generic;

namespace Core;

public partial class Cliente
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly DataNascimento { get; set; }

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual ICollection<Cartao> Cartaos { get; set; } = new List<Cartao>();

    public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
}

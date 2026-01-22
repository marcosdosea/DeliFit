using System;
using System.Collections.Generic;

namespace Core;

public partial class Restaurante
{
    public uint Id { get; set; }

    public string NomeRestaurante { get; set; } = null!;

    public string NomeProprietario { get; set; } = null!;

    public string CpfProprietario { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string? Descricao { get; set; }

    public string TelefoneProprietario { get; set; } = null!;

    public string TelefoneRestaurante { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte Validado { get; set; }

    public string Rua { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

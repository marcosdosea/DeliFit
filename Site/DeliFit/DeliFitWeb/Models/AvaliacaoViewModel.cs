using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models;

public class AvaliacaoViewModel
{
    public uint IdPedido { get; set; }
    public uint IdCliente { get; set; }

    [Required(ErrorMessage = "Selecione uma nota.")]
    [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
    public byte Nota { get; set; }

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }
}

public class ReclamacaoViewModel
{
    public uint IdPedido { get; set; }

    /// <summary>Motivo selecionado pelo cliente (preenchido via JS antes do submit).</summary>
    public string? Motivo { get; set; }

    [Required(ErrorMessage = "Descreva o problema ocorrido.")]
    [MinLength(10, ErrorMessage = "A descrição deve ter pelo menos 10 caracteres.")]
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;
}
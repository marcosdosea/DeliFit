using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Models;

public class ItemViewModel
{
    [Key]
    public uint Id { get; set; }

    [NotMapped]
    public byte[]? Foto { get; set; } = null;

    [Display(Name = "Quantidade de Calorias")]
    [Required(ErrorMessage = "Preenchimento do campo Quantidade de Calorias é obrigatório.")]
    public float Calorias { get; set; }

    [Display(Name = "Quantidade de Carboidratos")]
    public float? Carboidratos { get; set; }

    [Display(Name = "Quantidade de Gordura")]
    public float? Gordura { get; set; }

    [Display(Name = "Quantidade de Proteina")]
    public float? Proteina { get; set; }

    [Display(Name = "Restricao Alimentar")]
    [StringLength(50, ErrorMessage = "A restricao deve ter no máximo 50 caracteres.")]
    public string? Restricao { get; set; }

    [Display(Name = "Nome do Item")]
    [Required(ErrorMessage = "Preenchimento do campo Nome do Item é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string Nome { get; set; } = null!;

    [Display(Name = "Descricao")]
    [StringLength(50, ErrorMessage = "A descricao deve ter no máximo 200 caracteres.")]
    public string? Descricao { get; set; }

    [Display(Name = "Valor")]
    [Required(ErrorMessage = "Preenchimento do campo Valor é obrigatório.")]
    public decimal Preco { get; set; }

    [Display(Name = "Tamanho do Item")]
    public string? Tamanho { get; set; }

    [Display(Name = "Volume do Item")]
    [StringLength(50, ErrorMessage = "O volume deve ter no máximo 10 caracteres.")]
    public string? Volume { get; set; }

    [HiddenInput(DisplayValue = false)]

    [Required]
    public uint IdRestaurante { get; set; }
}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliFitWeb.Models;

public class ItemViewModel
{
    [Key]
    public uint Id { get; set; }

    /// <summary>
    /// Bytes da foto armazenada (usada para exibição nas views).
    /// </summary>
    public byte[]? Foto { get; set; } = null;

    /// <summary>
    /// Arquivo enviado pelo formulário. Não é mapeado pelo AutoMapper.
    /// </summary>
    [NotMapped]
    [Display(Name = "Foto do Item")]
    public IFormFile? FotoFile { get; set; }

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

    /// <summary>
    /// Restrições selecionadas no formulário (multi-seleção). Combinadas em <see cref="Restricao"/>
    /// (separadas por vírgula) antes de salvar; não mapeado pelo AutoMapper.
    /// </summary>
    [NotMapped]
    public List<string> RestricoesSelecionadas { get; set; } = new();

    [Display(Name = "Categorias")]
    public List<uint> CategoriaIds { get; set; } = new();

    /// <summary>
    /// Nomes das categorias do item, usados apenas para exibição (Details/Delete/listagens).
    /// </summary>
    [NotMapped]
    public List<string> CategoriaNomes { get; set; } = new();

    [Display(Name = "Nome do Item")]
    [Required(ErrorMessage = "Preenchimento do campo Nome do Item é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
    public string Nome { get; set; } = null!;

    [Display(Name = "Descricao")]
    [StringLength(200, ErrorMessage = "A descricao deve ter no máximo 200 caracteres.")]
    public string? Descricao { get; set; }

    [Display(Name = "Valor")]
    [Required(ErrorMessage = "Preenchimento do campo Valor é obrigatório.")]
    public decimal Preco { get; set; }

    [Display(Name = "Tamanho do Item")]
    public string? Tamanho { get; set; }

    [Display(Name = "Volume do Item")]
    [StringLength(10, ErrorMessage = "O volume deve ter no máximo 10 caracteres.")]
    public string? Volume { get; set; }

    [HiddenInput(DisplayValue = false)]
    [Required]
    public uint IdRestaurante { get; set; }
}
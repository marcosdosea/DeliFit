using System.ComponentModel.DataAnnotations;
using Util;

namespace DeliFitWeb.Models;

public class EnderecoViewModel
{
    [Key]
    public uint Id { get; set; }

    [Required]
    public uint IdCliente { get; set; }

    [Display(Name = "Rua")]
    [Required(ErrorMessage = "O campo Rua é obrigatório.")]
    [StringLength(50, ErrorMessage = "A rua deve ter no máximo 50 caracteres.")]
    public string Rua { get; set; } = string.Empty;

    [Display(Name = "Número")]
    [Required(ErrorMessage = "O campo Número é obrigatório.")]
    [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [Display(Name = "Bairro")]
    [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
    [StringLength(50, ErrorMessage = "O Bairro deve ter no máximo 50 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Display(Name = "CEP")]
    [Required(ErrorMessage = "O campo CEP é obrigatório.")]
    [StringLength(8, ErrorMessage = "O CEP deve ter no máximo 8 caracteres.")]
    [Cep(ErrorMessage = "CEP inválido")]
    public string Cep { get; set; } = string.Empty;

    [Display(Name = "Cidade")]
    [Required(ErrorMessage = "O campo Cidade é obrigatório.")]
    [StringLength(50, ErrorMessage = "A cidade deve ter no máximo 50 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    [Required(ErrorMessage = "O campo Estado é obrigatório.")]
    [StringLength(15, ErrorMessage = "O estado deve ter no máximo 15 caracteres.")]
    public string Estado { get; set; } = string.Empty;

    [Display(Name = "Label")]
    [StringLength(50, ErrorMessage = "O complemento deve ter no máximo 50 caracteres.")]
    public string Label { get; set; } = string.Empty;

}

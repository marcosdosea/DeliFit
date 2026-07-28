using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models;

public class CategoriaViewModel
{
    public uint Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = null!;

    public int QuantidadeItens { get; set; }
}

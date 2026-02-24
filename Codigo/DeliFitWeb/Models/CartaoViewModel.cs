using System.ComponentModel.DataAnnotations;
using Util;

namespace DeliFitWeb.Models
{
    public class CartaoViewModel
    {
        [Key]
        public uint Id { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "O número do cartão deve conter 16 caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "O CVV deve conter 3 caracteres.")]
        public string Cvv { get; set; } = string.Empty;

        [Display(Name = "Validade")]
        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Date, ErrorMessage = "Data válida requerida.")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Validade { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 caracteres.")]
        [CPF]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cliente obrigatório.")]
        public uint IdCliente { get; set; }
    }
}

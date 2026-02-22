using System.ComponentModel.DataAnnotations;
using Util;

namespace DeliFitWeb.Models
{
    public class ClienteViewModel
    {
        [Key]
        public uint Id { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve conter 11 caracteres.")]
        [TelefoneCelular(ErrorMessage = "Telefone inválido")]
        public string Telefone { get; set; } = string.Empty;


        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O cpf deve conter 11 caracteres.")]
        [CPF(ErrorMessage = "CPF inválido")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [StringLength(50, ErrorMessage = "O email deve ter no máximo 50 caracteres.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Ano Nascimento")]
        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.Date, ErrorMessage = "Data válida requirida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; } 


        

    }
}

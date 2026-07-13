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
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 caracteres.")]
        [CPF]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cliente obrigatório.")]
        public uint IdCliente { get; set; }

        /// <summary>
        /// Token de cartão gerado no navegador pelos Secure Fields do Mercado Pago
        /// (número/validade/CVV nunca chegam a este servidor). Preenchido pelo JS antes do submit.
        /// </summary>
        public string? Token { get; set; }

        // Campos somente leitura para exibição (Details/Index), vindos do cofre do Mercado Pago.
        public string Bandeira { get; set; } = string.Empty;

        public string UltimosQuatroDigitos { get; set; } = string.Empty;

        [Display(Name = "Validade")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Validade { get; set; }
    }
}
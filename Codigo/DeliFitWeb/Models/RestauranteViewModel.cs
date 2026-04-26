using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Util;

namespace DeliFitWeb.Models
{
    public class RestauranteViewModel
    {
        [Key]
        public uint Id { get; set; }

        public bool Validado { get; set; }

        /// <summary>
        /// Bytes da foto armazenada no banco (usado para exibição nas views).
        /// </summary>
        public byte[]? Foto { get; set; } = null;

        /// <summary>
        /// Arquivo enviado pelo formulário. Não é mapeado pelo AutoMapper.
        /// </summary>
        [NotMapped]
        [Display(Name = "Foto do Restaurante")]
        public IFormFile? FotoFile { get; set; }

        [Display(Name = "Nome do Restaurante")]
        [Required(ErrorMessage = "Preenchimento do campo Nome do Restaurante é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string NomeRestaurante { get; set; } = string.Empty;

        [Display(Name = "Nome do(a) Proprietário(a)")]
        [Required(ErrorMessage = "Preenchimento do campo Nome do Proprietário é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string NomeProprietario { get; set; } = string.Empty;

        [Display(Name = "CPF do(a) Proprietário(a)")]
        [Required(ErrorMessage = "O campo CPF do(a) Proprietário(a) é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 caracteres.")]
        [CPF(ErrorMessage = "CPF inválido")]
        public string CpfProprietario { get; set; } = string.Empty;

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O campo CNPJ é obrigatório.")]
        [StringLength(14, ErrorMessage = "O CNPJ deve ter no máximo 14 caracteres.")]
        [CNPJ(ErrorMessage = "CNPJ inválido")]
        public string Cnpj { get; set; } = string.Empty;

        [Display(Name = "Descrição do Restaurante")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string? Descricao { get; set; } = null;

        [Display(Name = "Telefone do(a) Proprietário(a)")]
        [Required(ErrorMessage = "O campo Telefone do(a) Proprietário(a) é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve conter 11 caracteres.")]
        [TelefoneCelular(ErrorMessage = "Telefone inválido")]
        public string TelefoneProprietario { get; set; } = string.Empty;

        [Display(Name = "Telefone do Restaurante")]
        [Required(ErrorMessage = "O campo Telefone do Restaurante é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve conter 11 caracteres.")]
        [TelefoneCelular(ErrorMessage = "Telefone inválido")]
        public string TelefoneRestaurante { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [StringLength(50, ErrorMessage = "O email deve ter no máximo 50 caracteres.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; } = string.Empty;

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
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve conter 8 caracteres.")]
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
    }
}
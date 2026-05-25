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
        /// Bytes da foto armazenada no banco (usado para exibi��o nas views).
        /// </summary>
        public byte[]? Foto { get; set; } = null;

        /// <summary>
        /// Arquivo enviado pelo formul�rio. N�o � mapeado pelo AutoMapper.
        /// </summary>
        [NotMapped]
        [Display(Name = "Foto do Restaurante")]
        public IFormFile? FotoFile { get; set; }

        [Display(Name = "Nome do Restaurante")]
        [Required(ErrorMessage = "Preenchimento do campo Nome do Restaurante � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no m�ximo 50 caracteres.")]
        public string NomeRestaurante { get; set; } = string.Empty;

        [Display(Name = "Nome do(a) Propriet�rio(a)")]
        [Required(ErrorMessage = "Preenchimento do campo Nome do Propriet�rio � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no m�ximo 50 caracteres.")]
        public string NomeProprietario { get; set; } = string.Empty;

        [Display(Name = "CPF do(a) Propriet�rio(a)")]
        [Required(ErrorMessage = "O campo CPF do(a) Propriet�rio(a) � obrigat�rio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 caracteres.")]
        [CPF(ErrorMessage = "CPF inv�lido")]
        public string CpfProprietario { get; set; } = string.Empty;

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O campo CNPJ � obrigat�rio.")]
        [StringLength(14, ErrorMessage = "O CNPJ deve ter no m�ximo 14 caracteres.")]
        [CNPJ(ErrorMessage = "CNPJ inv�lido")]
        public string Cnpj { get; set; } = string.Empty;

        [Display(Name = "Descri��o do Restaurante")]
        [StringLength(200, ErrorMessage = "A descri��o deve ter no m�ximo 200 caracteres.")]
        public string? Descricao { get; set; } = null;

        [Display(Name = "Telefone do(a) Propriet�rio(a)")]
        [Required(ErrorMessage = "O campo Telefone do(a) Propriet�rio(a) � obrigat�rio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve conter 11 caracteres.")]
        [TelefoneCelular(ErrorMessage = "Telefone inv�lido")]
        public string TelefoneProprietario { get; set; } = string.Empty;

        [Display(Name = "Telefone do Restaurante")]
        [Required(ErrorMessage = "O campo Telefone do Restaurante � obrigat�rio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve conter 11 caracteres.")]
        [TelefoneCelular(ErrorMessage = "Telefone inv�lido")]
        public string TelefoneRestaurante { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "O campo Email � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "O email deve ter no m�ximo 50 caracteres.")]
        [EmailAddress(ErrorMessage = "O email informado n�o � v�lido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Rua")]
        [Required(ErrorMessage = "O campo Rua � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "A rua deve ter no m�ximo 50 caracteres.")]
        public string Rua { get; set; } = string.Empty;

        [Display(Name = "N�mero")]
        [Required(ErrorMessage = "O campo N�mero � obrigat�rio.")]
        [StringLength(10, ErrorMessage = "O n�mero deve ter no m�ximo 10 caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "O campo Bairro � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "O Bairro deve ter no m�ximo 50 caracteres.")]
        public string Bairro { get; set; } = string.Empty;

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "O campo CEP � obrigat�rio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve conter 8 caracteres.")]
        [Cep(ErrorMessage = "CEP inv�lido")]
        public string Cep { get; set; } = string.Empty;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo Cidade � obrigat�rio.")]
        [StringLength(50, ErrorMessage = "A cidade deve ter no m�ximo 50 caracteres.")]
        public string Cidade { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "O campo Estado � obrigat�rio.")]
        [StringLength(15, ErrorMessage = "O estado deve ter no m�ximo 15 caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [NotMapped]
        public double MediaAvaliacao { get; set; } = 0.0;
    }
}
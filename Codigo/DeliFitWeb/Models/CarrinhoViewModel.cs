using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models
{
    public class CarrinhoViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Observação")]
        [StringLength(255, ErrorMessage = "A observação deve ter no máximo 255 caracteres.")]
        public string? Observacao { get; set; }

        [Required]
        public uint IdCliente { get; set; }

        [Required]
        [Display(Name = "Forma de Pagamento")]
        [RegularExpression("[PCD]", ErrorMessage = "Use P (PIX), C (Cartão) ou D (Dinheiro).")]
        public string FormaDePagamento { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O valor do frete deve ser positivo.")]
        [Display(Name = "Valor do Frete")]
        public decimal ValorFrete { get; set; }

        [Display(Name = "Cartão")]
        public uint? IdCartao { get; set; }

        public List<uint> IdsPedidos { get; set; } = [];

        public ClienteViewModel? Cliente { get; set; }

        public CartaoViewModel? Cartao { get; set; }

  
    }
}

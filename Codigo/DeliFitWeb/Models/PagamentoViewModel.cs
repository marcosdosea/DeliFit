using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models
{
    public class PagamentoViewModel
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        public uint idRestaurante { get; set; }

        [Required]
        [Display(Name = "Valor Mensalidade")]
        public decimal ValorMensalidade { get; set; } = 0;

        [Required]
        [Display(Name = "Data Pagamento")]
        public DateTime? DataPagamento { get; set; } = null;

        [Required]
        [Display(Name = "Data Vencimento")]
        public DateTime? DataVencimento { get; set; } = null;

        [Required]
        [Display(Name = "Status Mensalidade")]
        public string StatusMensalidade { get; set; } = string.Empty;


    }
}

using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models
{
    public class PedidoViewModel
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        public uint IdCarrinho { get; set; }

        [Required]
        public uint IdRestaurante { get; set; }

        [Required]
        [Display(Name = "Data do Pedido")]
        public DateTime? Data { get; set; }
        
        [Required]
        [Display(Name = "Preço do pedido")]
        public decimal Preco { get; set; } = 0;

        //ponto de analise quando inserir a avaliacao view model
        //public AvaliacaoViewModel? Avaliacao { get; set; }

        

    }
}

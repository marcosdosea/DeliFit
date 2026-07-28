using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models;

public class FaturamentoViewModel
{
    [Key]
    public uint IdRestaurante { get; set; }

    [Display(Name = "Data")]
    public DateTime Data { get; set; }

    [Display(Name = "Faturamento Total")]
    public decimal TotalFaturamento { get; set; }

    [Display(Name = "Total de Pedidos")]
    public int TotalPedidos { get; set; }
}

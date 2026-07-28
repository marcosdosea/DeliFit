namespace Core.DTO;

public class FaturamentoDTO
{
    public uint IdRestaurante { get; set; }

    public DateTime Data { get; set; }

    public decimal TotalFaturamento { get; set; }

    public int TotalPedidos { get; set; }

    public List<Pedido> Pedidos { get; set; }
}

namespace DeliFitWeb.Models
{
    public class AtualizarStatusPedidoRequest
    {
        public uint PedidoId { get; set; }
        public int NovoStatus { get; set; }
    }
}

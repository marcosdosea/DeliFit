namespace Core.Service
{
    public interface IPedidoService
    {
        public uint Create(Pedido pedido);
        public IEnumerable<Pedido> GetAll();
        public Pedido? Get(uint id);
        public void Delete(uint id);
    }
}
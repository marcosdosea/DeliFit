namespace Core.Service
{
    public interface IPedidoService
    {
        uint Create(Pedido pedido);
        public IEnumerable<Pedido> GetAll();
        Pedido? Get(uint id);
        void Delete(uint id);
    }
}
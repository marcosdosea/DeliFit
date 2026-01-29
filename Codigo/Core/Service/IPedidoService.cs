namespace Core.Service
{
    public interface IPedidoService
    {
        uint Create(Pedido pedido);
        IEnumerable<Pedido> GetAll();
        Pedido? Get(uint id);
        void Delete(uint id);
    }
}
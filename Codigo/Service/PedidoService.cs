using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PedidoService : IPedidoService
    {
        private readonly DeliFitContext _context;

        public PedidoService(DeliFitContext context)
        {
            _context = context;
        }

        public uint Create(Pedido pedido)
        {
            _context.Add(pedido);
            _context.SaveChanges();
            return pedido.Id;
        }

        public IEnumerable<Pedido> GetAll()
        {
            return _context.Pedidos.AsNoTracking();
        }

        public Pedido? Get(uint id)
        {
            return _context.Pedidos.FirstOrDefault(a => a.Id == id);
        }

        public void Delete(uint id)
        {
            var pedido = _context.Pedidos.Find(id);
            if (pedido != null)
            {
                _context.Remove(pedido);
                _context.SaveChanges();
            }
        }


    }
}

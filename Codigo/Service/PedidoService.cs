using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    /// <summary>
    /// Implementa os serviços para manter os dados de pedidos
    /// </summary>
    public class PedidoService : IPedidoService
    {
        private readonly DeliFitContext _context;

        public PedidoService(DeliFitContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo pedido na base de dados
        /// </summary>
        /// <param name="pedido">dados do pedido</param>
        /// <returns>id do novo pedido</returns>
        public uint Create(Pedido pedido)
        {
            _context.Add(pedido);
            _context.SaveChanges();
            return pedido.Id;
        }

        /// <summary>
        /// Obter dados de todos os pedidos da base de dados
        /// </summary>
        /// <returns>dados dos pedidos</returns>
        public IEnumerable<Pedido> GetAll()
        {
            return _context.Pedidos.AsNoTracking();
        }

        /// <summary>
        /// Obter os dados de um pedido da base de dados
        /// </summary>
        /// <param name="id">id do pedido</param>
        /// <returns>dados do pedido</returns>
        public Pedido? Get(uint id)
        {
            return _context.Pedidos.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Remover dados de um pedido da base de dados
        /// </summary>
        /// <param name="id">id do pedido</param>
        public void Delete(uint id)
        {
            var pedido = _context.Pedidos.Find(id);
            if (pedido != null)
            {
                _context.Remove(pedido);
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Pedido não encontrado");
            }
        }


    }
}

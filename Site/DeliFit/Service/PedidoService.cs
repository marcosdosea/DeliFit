using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

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
        return _context.Pedidos
            .Include(p => p.Pedidoitems)
            .ThenInclude(pi => pi.IdItemNavigation)
            .FirstOrDefault(a => a.Id == id);
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
            // Remove os PedidoItems primeiro (FK constraint)
            _context.Entry(pedido).Collection(p => p.Pedidoitems).Load();
            _context.Pedidoitems.RemoveRange(pedido.Pedidoitems);

            _context.Remove(pedido);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Pedido não encontrado");
        }
    }

    /// <summary>
    /// Atualizar dados de um pedido na base de dados
    /// </summary>
    /// <param name="pedido">dados atualizados do pedido</param>
    public void Edit(Pedido pedido)
    {
        var existente = _context.Pedidos.FirstOrDefault(p => p.Id == pedido.Id);
        if (existente == null)
            throw new ServiceException("Pedido não encontrado");

        existente.Status = pedido.Status;
        existente.Data = pedido.Data;
        existente.Preco = pedido.Preco;

        _context.SaveChanges();
    }


}

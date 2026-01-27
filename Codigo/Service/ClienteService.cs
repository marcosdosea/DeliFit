using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class ClienteService : IClienteService
{
    private readonly DeliFitContext _context;

    public ClienteService(DeliFitContext context)
    {
        _context = context;

    }

    public uint Create(Cliente cliente)
    {
        _context.Add(cliente);
        _context.SaveChanges();
        return cliente.Id;
    }

    public Cliente? Get(uint id)
    {
        return _context.Clientes
                    .FirstOrDefault
                    (a => a.Id == id);
    }

    public void Edit(Cliente cliente)
    {

        _context.Update(cliente);
        _context.SaveChanges();

    }

    public void Delete(uint id)
    {
        var Cliente = _context.Clientes.FirstOrDefault(a => a.Id == id);
        if (Cliente != null)
        {
            _context.Remove(Cliente);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Cliente não encontrado");
        }
    }

    public IEnumerable<Cliente> GetAll()
    {
        return _context.Clientes
            .OrderBy(a => a.Id)
            .AsNoTracking();
    }

}

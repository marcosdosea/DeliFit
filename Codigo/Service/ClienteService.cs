using Core;
using Core.Service;

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
       return _context.Clientes.Find(id);
    }

    public void Edit(Cliente cliente)
    {
        if(Get(cliente.Id) != null)
        {
            _context.Update(cliente);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Cliente não encontrado");
        }
    }

    public void Delete(uint id)
    {
        var Cliente = _context.Clientes.FirstOrDefault(a => a.Id == id);
        if(Cliente != null)
        {
            _context.Remove(Cliente);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Cliente não encontrado");
        }
    }

}

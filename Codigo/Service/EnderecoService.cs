using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class EnderecoService : IEnderecoService
{
    private readonly DeliFitContext _context;

    public EnderecoService(DeliFitContext context)
    {
        _context = context;

    }

    public uint Create(Endereco endereco)
    {
        _context.Add(endereco);
        _context.SaveChanges();
        return endereco.Id;
    }

    public Endereco? Get(uint id)
    {
        return _context.Enderecos.FirstOrDefault(a => a.Id == id);
    }

    public void Edit(Endereco endereco)
    {
        _context.Update(endereco);
        _context.SaveChanges();
    }

    public void Delete(uint id)
    {
        var Endereco = _context.Enderecos.FirstOrDefault(a => a.Id == id);
        if (Endereco != null)
        {
            _context.Remove(Endereco);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Endereço não encontrado");
        }
    }

    public IEnumerable<Endereco> GetAll()
    {
        return _context.Enderecos.AsNoTracking();
    }

}

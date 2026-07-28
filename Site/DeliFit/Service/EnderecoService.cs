using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os dados de enderecos
/// </summary>
public class EnderecoService : IEnderecoService
{
    private readonly DeliFitContext _context;

    public EnderecoService(DeliFitContext context)
    {
        _context = context;

    }

    /// <summary>
    /// Cria um novo endereco na base de dados
    /// </summary>
    /// <param name="endereco">dados do endereco</param>
    /// <returns>id do novo endereco</returns>
    public uint Create(Endereco endereco)
    {
        _context.Add(endereco);
        _context.SaveChanges();
        return endereco.Id;
    }

    /// <summary>
    /// Obter os dados de um endereco da base de dados
    /// </summary>
    /// <param name="id">id do endereco</param>
    /// <returns>dados do endereco</returns>
    public Endereco? Get(uint id)
    {
        return _context.Enderecos.FirstOrDefault(a => a.Id == id);
    }

    /// <summary>
    /// Atualizar dados de um endereco na base de dados
    /// </summary>
    /// <param name="endereco">novos dados do endereco</param>
    public void Edit(Endereco endereco)
    {
        _context.Update(endereco);
        _context.SaveChanges();
    }

    /// <summary>
    /// Remover dados de um endereco da base de dados
    /// </summary>
    /// <param name="id">id do endereco</param>
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

    /// <summary>
    /// Obter dados de todos os enderecos da base de dados
    /// </summary>
    /// <returns>dados dos enderecos</returns>
    public IEnumerable<Endereco> GetAll()
    {
        return _context.Enderecos.AsNoTracking();
    }

}

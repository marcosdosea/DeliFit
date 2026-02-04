using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os dados de Cartaos
/// </summary>
public class CartaoService : ICartaoService
{
    private readonly DeliFitContext _context;

    public CartaoService(DeliFitContext context)
    {
        _context = context;

    }

    /// <summary>
    /// Cria um novo Cartao na base de dados
    /// </summary>
    /// <param name="Cartao">dados do Cartao</param>
    /// <returns>id do novo Cartao</returns>
    public uint Create(Cartao Cartao)
    {
        _context.Add(Cartao);
        _context.SaveChanges();
        return Cartao.Id;
    }

    /// <summary>
    /// Obter os dados de um Cartao da base de dados
    /// </summary>
    /// <param name="id">id do Cartao</param>
    /// <returns>dados do Cartao</returns>
    public Cartao? Get(uint id)
    {
        return _context.Cartaos
                    .FirstOrDefault
                    (a => a.Id == id);
    }

    /// <summary>
    /// Atualizar dados de um Cartao da base de dados
    /// </summary>
    /// <param name="Cartao">novos dados do Cartao</param>
    public void Edit(Cartao Cartao)
    {

        _context.Update(Cartao);
        _context.SaveChanges();

    }

    /// <summary>
    /// Remover dados de um Cartao da base de dados
    /// </summary>
    /// <param name="id">id do Cartao</param>
    public void Delete(uint id)
    {
        var Cartao = _context.Cartaos.FirstOrDefault(a => a.Id == id);
        if (Cartao != null)
        {
            _context.Cartaos.Remove(Cartao);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Cartao não encontrado");
        }
    }

    /// <summary>
    /// Obter dados de todos os Cartaos da base de dados
    /// </summary>
    /// <returns>dados dos Cartaos</returns>
    public IEnumerable<Cartao> GetAll()
    {
        return _context.Cartaos
            .ToList();
    }

}





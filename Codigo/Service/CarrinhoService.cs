using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os dados de Carrinhos
/// </summary>
public class CarrinhoService : ICarrinhoService
{
    private readonly DeliFitContext _context;

    public CarrinhoService(DeliFitContext context)
    {
        _context = context;

    }

    /// <summary>
    /// Cria um novo Carrinho na base de dados
    /// </summary>
    /// <param name="Carrinho">dados do Carrinho</param>
    /// <returns>id do novo Carrinho</returns>
    public uint Create(Carrinho Carrinho)
    {
        _context.Add(Carrinho);
        _context.SaveChanges();
        return Carrinho.Id;
    }

    /// <summary>
    /// Obter os dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="id">id do Carrinho</param>
    /// <returns>dados do Carrinho</returns>
    public Carrinho? Get(uint id)
    {
        return _context.Carrinhos
                    .FirstOrDefault
                    (a => a.Id == id);
    }

    /// <summary>
    /// Atualizar dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="Carrinho">novos dados do Carrinho</param>
    public void Edit(Carrinho Carrinho)
    {

        _context.Update(Carrinho);
        _context.SaveChanges();

    }

    /// <summary>
    /// Remover dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="id">id do Carrinho</param>
    public void Delete(uint id)
    {
        var Carrinho = _context.Carrinhos.FirstOrDefault(a => a.Id == id);
        if (Carrinho != null)
        {
            _context.Carrinhos.Remove(Carrinho);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Carrinho não encontrado");
        }
    }

    /// <summary>
    /// Obter dados de todos os Carrinhos da base de dados
    /// </summary>
    /// <returns>dados dos Carrinhos</returns>
    public IEnumerable<Carrinho> GetAll()
    {
        return _context.Carrinhos
            .ToList();
    }

}





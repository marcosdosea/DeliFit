using Core;
using Core.DTO;
using Core.Service;

namespace Service;

/// <summary>
/// Implementar os serviços para manter os dados de categorias
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly DeliFitContext _context;

    public CategoriaService(DeliFitContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obter a lista das categorias disponiveis no sistema, com a quantidade de itens em cada uma
    /// </summary>
    /// <returns>retorna todas as categorias registradas</returns>
    public IEnumerable<CategoriaDTO> ListarCategorias()
    {
        return _context.Categorias
            .Select(c => new CategoriaDTO
            {
                Nome = c.Nome,
                QuantidadeItens = c.Items.Count
            })
            .ToList();
    }

    /// <summary>
    /// Obter a lista de itens por categoria
    /// </summary>
    /// <param name="categoria"></param>
    /// <returns>retorna todos os itens da categoria especificada</returns>
    public IEnumerable<Item> ListarItensPorCategoria(string categoria)
    {
        return _context.Items
            .Where(i => i.Categorias.Any(c => c.Nome == categoria))
            .ToList();
    }

    /// <summary>
    /// Obter todas as categorias cadastradas (id + nome), usadas para montar a seleção de categorias de um item
    /// </summary>
    public IEnumerable<Categoria> ListarTodas()
    {
        return _context.Categorias
            .OrderBy(c => c.Nome)
            .ToList();
    }
}

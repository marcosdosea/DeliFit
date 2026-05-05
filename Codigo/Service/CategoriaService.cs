using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

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
    /// Obter a lista das categorias disponiveis no sistema
    /// </summary>
    /// <returns>retorna todas as categorias registradas</returns>
    // Categorias fixas do sistema DeliFit
    private static readonly List<string> _categoriasPadrao = new()
    {
        "Vegetariano", "Vegano", "Sem Glúten", "Sem Lactose",
        "Fitness", "Low Carb", "Zero Lactose", "Proteico"
    };

    public IEnumerable<CategoriaDTO> ListarCategorias()
    {
        // Conta quantos itens existem para cada categoria fixa
        var contagemPorCategoria = _context.Items
            .Where(i => i.Restricao != null && i.Restricao != "")
            .GroupBy(i => i.Restricao)
            .Select(g => new { Nome = g.Key, Qtd = g.Count() })
            .ToList();

        return _categoriasPadrao.Select(cat => new CategoriaDTO
        {
            Nome = cat,
            QuantidadeItens = contagemPorCategoria
                .FirstOrDefault(c => c.Nome == cat)?.Qtd ?? 0
        }).ToList();
    }


    /// <summary>
    /// Obter a lista de itens por categoria
    /// </summary>
    /// <param name="categoria"></param>
    /// <returns>retorna todos os itens da categoria especificada</returns>
    public IEnumerable<Item> ListarItensPorCategoria(string categoria)
    {

        return _context.Items
            .Where(i => i.Restricao == categoria)
            .ToList();
    }


}

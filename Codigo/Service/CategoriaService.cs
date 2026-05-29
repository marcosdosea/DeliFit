using Core;
using Core.DTO;
using Core.Service;

namespace Service;

public class CategoriaService : ICategoriaService
{
    private readonly DeliFitContext _context;

    public CategoriaService(DeliFitContext context)
    {
        _context = context;
    }

    public IEnumerable<CategoriaDTO> ListarCategorias()
    {
        var categorias = _context.Categorias.OrderBy(c => c.Nome).ToList();

        var contagemPorCategoria = _context.Items
            .Where(i => i.Restricao != null && i.Restricao != "")
            .GroupBy(i => i.Restricao)
            .Select(g => new { Nome = g.Key, Qtd = g.Count() })
            .ToList();

        return categorias.Select(cat => new CategoriaDTO
        {
            Id = cat.Id,
            Nome = cat.Nome,
            QuantidadeItens = contagemPorCategoria
                .FirstOrDefault(c => c.Nome == cat.Nome)?.Qtd ?? 0
        }).ToList();
    }

    public IEnumerable<Item> ListarItensPorCategoria(string categoria)
    {
        return _context.Items
            .Where(i => i.Restricao == categoria)
            .ToList();
    }

    public void Create(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ServiceException("O nome da categoria não pode ser vazio.");

        nome = nome.Trim();

        if (_context.Categorias.Any(c => c.Nome == nome))
            throw new ServiceException("Já existe uma categoria com esse nome.");

        _context.Categorias.Add(new Categoria { Nome = nome });
        _context.SaveChanges();
    }

    public void Delete(uint id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria == null)
            throw new ServiceException("Categoria não encontrada.");

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
    }

    public Categoria? Get(uint id)
    {
        return _context.Categorias.Find(id);
    }
}

using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os dados de itens
/// </summary>
public class ItemService : IItemService
{
    private readonly DeliFitContext _context;

    public ItemService(DeliFitContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo item na base de dados
    /// </summary>
    /// <param name="item">dados do item</param>
    /// <returns>id do novo item</returns>
    public uint Create(Item item, IEnumerable<uint>? categoriaIds = null)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        var restauranteExists = _context.Restaurantes.Any(r => r.Id == item.IdRestaurante);
        if (!restauranteExists)
            throw new ServiceException($"Restaurante com ID {item.IdRestaurante} não encontrado.");

        if (categoriaIds != null)
        {
            var ids = categoriaIds.ToList();
            item.Categorias = _context.Categorias.Where(c => ids.Contains(c.Id)).ToList();
        }

        _context.Add(item);
        _context.SaveChanges();
        return item.Id;
    }

    /// <summary>
    /// Remover dados de um item da base de dados
    /// </summary>
    /// <param name="id">id do item</param>
    public void Delete(uint id)
    {
        var item = _context.Items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            _context.Remove(item);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Item não encontrado");
        }
    }

    /// <summary>
    /// Atualizar dados de um item da base de dados
    /// </summary>
    /// <param name="item">novos dados do item</param>
    public void Edit(Item item, IEnumerable<uint>? categoriaIds = null)
    {
        var existingItem = _context.Items
            .Include(i => i.IdRestauranteNavigation)
            .Include(i => i.Categorias)
            .FirstOrDefault(i => i.Id == item.Id);

        if (existingItem == null)
        {
            throw new Exception($"Item com ID {item.Id} não encontrado");
        }

        _context.Entry(existingItem).CurrentValues.SetValues(item);

        if (categoriaIds != null)
        {
            var ids = categoriaIds.ToList();
            var novasCategorias = _context.Categorias.Where(c => ids.Contains(c.Id)).ToList();

            existingItem.Categorias.Clear();
            foreach (var categoria in novasCategorias)
                existingItem.Categorias.Add(categoria);
        }

        _context.SaveChanges();
    }

    /// <summary>
    /// Obter os dados de um item da base de dados
    /// </summary>
    /// <param name="id">id do item</param>
    /// <returns>dados do item</returns>
    public Item? Get(uint id)
    {
        return _context.Items
            .Include(i => i.Categorias)
            .FirstOrDefault(i => i.Id == id);
    }

    /// <summary>
    /// Obter dados de todos os itens da base de dados
    /// </summary>
    /// <returns>dados dos itens</returns>
    public IEnumerable<Item> GetAll()
    {
        return _context.Items
            .Include(i => i.Categorias)
            .AsNoTracking();
    }


    /// <summary>
    /// Obter dados dos itens que iniciam com um nome
    /// </summary>
    /// <param name="nome">nome a ser buscado</param>
    /// <returns>lista de itens</returns>
    public IEnumerable<Item> GetByName(string nome)
    {
        return _context.Items
            .Include(i => i.Categorias)
            .AsNoTracking()
            .Where(i => EF.Functions.Like(i.Nome, $"%{nome}%"));//ignora maiuscula e minuscula
    }

    /// <summary>
    /// Obter todos os itens de um restaurante específico
    /// </summary>
    /// <param name="idRestaurante">ID do restaurante</param>
    /// <returns>lista de itens do restaurante</returns>
    public IEnumerable<Item> GetByRestaurante(uint idRestaurante)
    {
        return _context.Items
            .Include(i => i.Categorias)
            .AsNoTracking()
            .Where(i => i.IdRestaurante == idRestaurante)
            .OrderBy(i => i.Nome);
    }
}
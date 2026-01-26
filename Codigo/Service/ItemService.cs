using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class ItemService : IItemService
{
    private readonly DeliFitContext _context;

    public ItemService(DeliFitContext context)
    {
        _context = context;
    }

    public uint Create(Item item)
    {
        _context.Add(item);
        _context.SaveChanges();
        return item.Id;
    }

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

    public void Edit(Item item)
    {
        if (Get(item.Id) != null)
        {
            _context.Items.Update(item);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Item não encontrado");
        }

    }

    public Item? Get(uint id)
    {
        return _context.Items.Find(id);
    }

    public IEnumerable<Item> GetAll()
    {
        return _context.Items.AsNoTracking();
    }

    public IEnumerable<Item> GetByName(string nome)
    {
        return _context.Items
            .AsNoTracking()
            .Where(i => EF.Functions.Like(i.Nome, $"%{nome}%"));//ignora maiuscula e minuscula
    }
}
namespace Core.Service;

public interface IItemService
{
    public uint Create(Item item);

    public void Edit(Item item);

    public void Delete(uint id);

    public Item? Get(uint id);

    IEnumerable<Item> GetAll();

    IEnumerable<Item> GetByName(string nome);
}

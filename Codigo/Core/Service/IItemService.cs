namespace Core.Service;

public interface IItemService
{
    public uint Create(Item item);

    public void Edit(Item item);

    public void Delete(uint id);

    public Item? Get(uint id);

    public IEnumerable<Item> GetAll();
}

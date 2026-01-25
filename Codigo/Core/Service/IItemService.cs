namespace Core.Service;

public interface IItemService
{
    uint Create(Item item);

    void Edit(Item item);

    void Delete(uint id);

    Item? Get(uint id);

    IEnumerable<Item> GetAll();

    IEnumerable<Item> GetByName(string nome);
}

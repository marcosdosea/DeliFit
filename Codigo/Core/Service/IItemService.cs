namespace Core.Service;

public interface IItemService
{
    uint Create(Item item, IEnumerable<uint>? categoriaIds = null);

    void Edit(Item item, IEnumerable<uint>? categoriaIds = null);

    void Delete(uint id);

    Item? Get(uint id);

    IEnumerable<Item> GetAll();

    IEnumerable<Item> GetByName(string nome);

    IEnumerable<Item> GetByRestaurante(uint idRestaurante);
}

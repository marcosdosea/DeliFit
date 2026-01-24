namespace Core.Service;

public interface IRestauranteService
{
    uint Create(Restaurante restaurante);

    void Delete(uint id);

    void Edit(Restaurante restaurante);

    Restaurante? Get(uint id);
}

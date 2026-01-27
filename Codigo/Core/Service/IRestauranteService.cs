using Core.DTO;

namespace Core.Service;

public interface IRestauranteService
{
    public uint Create(Restaurante restaurante);

    public void Delete(uint id);

    public void Edit(Restaurante restaurante);

    public Restaurante? Get(uint id);

    public IEnumerable<RestauranteDTO> GetAll();
}

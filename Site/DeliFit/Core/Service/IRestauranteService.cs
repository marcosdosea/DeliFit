using Core.DTO;

namespace Core.Service;

public interface IRestauranteService
{
    uint Create(Restaurante restaurante);

    void Delete(uint id);

    void Edit(Restaurante restaurante);

    Restaurante? Get(uint id);

    IEnumerable<RestauranteDTO> GetAll();

    IEnumerable<FaturamentoDTO> GetAllFaturamentos(uint idRestaurante, DateTime? dataInicio = null, DateTime? dataFim = null);
    IEnumerable<RestauranteDTO> GetRestaurantesAtivos();
    IEnumerable<RestauranteDTO> GetRestaurantesPendentes();
    Restaurante? GetByEmail(string email);

    /// <summary>Busca restaurantes ativos cujo nome contenha o termo.</summary>
    IEnumerable<RestauranteDTO> Buscar(string termo);

    /// <summary>Retorna restaurantes ativos que possuem itens com a restrição/categoria indicada.</summary>
    IEnumerable<RestauranteDTO> GetByRestricao(string restricao);
}

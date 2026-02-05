using Core.DTO;

namespace Core.Service
{
    public interface IFaturamentoService
    {
        FaturamentoDTO? Get(uint idRestaurante, DateTime dataInicio, DateTime dataFim);

        IEnumerable<FaturamentoDTO> GetAll(uint idRestaurante);
    }
}

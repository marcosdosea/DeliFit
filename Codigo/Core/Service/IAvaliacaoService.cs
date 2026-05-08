using Core;

namespace Core.Service
{
    public interface IAvaliacaoService
    {
        void Create(Avaliacao avaliacao);
        Avaliacao? Get(uint id);
        IEnumerable<Avaliacao> GetAll();
        void Update(Avaliacao avaliacao);
        void Delete(uint id);
        IEnumerable<Avaliacao> GetByPedido(uint idPedido);
        IEnumerable<Avaliacao> GetByCliente(uint idCliente);
    }
}
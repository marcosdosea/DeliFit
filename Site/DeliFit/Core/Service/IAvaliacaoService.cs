namespace Core.Service;

public interface IAvaliacaoService
{
    void Create(Core.Avaliacao avaliacao);
    void Edit(Core.Avaliacao avaliacao);
    void Delete(uint id);
    Core.Avaliacao? Get(uint id);
    IEnumerable<Core.Avaliacao> GetAll();
    IEnumerable<Core.Avaliacao> GetByCliente(uint idCliente);

    /// <summary>Retorna a avaliação de um pedido específico, ou null se ainda não avaliado.</summary>
    Core.Avaliacao? GetByPedido(uint idPedido);
}
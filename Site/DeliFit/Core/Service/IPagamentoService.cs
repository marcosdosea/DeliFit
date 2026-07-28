namespace Core.Service;

public interface IPagamentoService
{
    uint Create(Pagamento pagamento);

    Pagamento? Get(uint id);

    IEnumerable<Pagamento> GetAll();

    IEnumerable<Pagamento> GetAllByRestaurante(uint idRestaurante);

    IEnumerable<Pagamento> GetAllByStatus(string statusMensalidade);
}

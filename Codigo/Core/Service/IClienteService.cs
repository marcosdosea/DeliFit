using Core.DTO;

namespace Core.Service;

public interface IClienteService
{
    uint Create(Cliente cliente);

    Cliente? Get(uint id);

    void Edit(Cliente cliente);

    void Delete(uint id);

    IEnumerable<ClienteDTO> GetAll();

    /// <summary>
    /// Busca um cliente pelo e-mail para autenticação
    /// </summary>
    Cliente? GetByEmail(string email);

    /// <summary>
    /// Busca um cliente pelo telefone para autenticação
    /// </summary>
    Cliente? GetByTelefone(string telefone);

    /// <summary>
    /// Busca um cliente pelo CPF
    /// </summary>
    Task<Cliente?> GetByCpf(string cpf);
}
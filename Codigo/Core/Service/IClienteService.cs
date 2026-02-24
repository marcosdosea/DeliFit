using Core.DTO;

namespace Core.Service;

public interface IClienteService
{
    //esses dois se referem ao caso de uso autenticar cliente
    uint Create(Cliente cliente);

    Cliente? Get(uint id);

    //esses dois metodos sao para o caso de uso manter perfil
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
}
namespace Core.Service;

public interface IClienteService
{
    //esses dois se referem ao caso de uso autenticar cliente
    uint Create(Cliente cliente);

    Cliente Get(uint id);

    //esses dois metodos sao para o caso de uso manter perfil
    void Edit(Cliente cliente);

    void Delete(uint id);
}

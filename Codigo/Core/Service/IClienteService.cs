namespace Core.Service;

public interface IClienteService
{
    //esses dois se referem ao caso de uso autenticar cliente
    public uint Create(Cliente cliente);

    public Cliente? Get(uint id);

    //esses dois metodos sao para o caso de uso manter perfil
    public void Edit(Cliente cliente);

     public void Delete(uint id);

    public IEnumerable<Cliente> GetAll();
}

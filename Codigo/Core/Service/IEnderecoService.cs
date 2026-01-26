namespace Core.Service
{
    public interface IEnderecoService
    {
        uint Create(Endereco endereco);
        Endereco? Get(uint id);
        void Edit(Endereco endereco);
        void Delete(uint id);
        IEnumerable<Endereco> GetAll();
    }
}

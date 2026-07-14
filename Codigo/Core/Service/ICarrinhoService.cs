namespace Core.Service;

public interface ICarrinhoService
{
    public uint Create(Carrinho Carrinho);

    public Carrinho? Get(uint id);

    public void Edit(Carrinho Carrinho);

    public void Delete(uint id);

    public void ValidarCarrinho(Carrinho Carrinho);

    public IEnumerable<Carrinho> GetAll();
}

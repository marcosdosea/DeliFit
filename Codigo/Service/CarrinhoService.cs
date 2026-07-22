using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os dados de Carrinhos
/// </summary>
public class CarrinhoService : ICarrinhoService
{
    private readonly DeliFitContext _context;

    public CarrinhoService(DeliFitContext context)
    {
        _context = context;

    }

    /// <summary>
    /// Cria um novo Carrinho na base de dados
    /// </summary>
    /// <param name="Carrinho">dados do Carrinho</param>
    /// <returns>id do novo Carrinho</returns>
    public uint Create(Carrinho carrinho)
    {
        ValidarCarrinho(carrinho);


        _context.Add(carrinho);
        _context.SaveChanges();
        return carrinho.Id;
    }

    /// <summary>
    /// Obter os dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="id">id do Carrinho</param>
    /// <returns>dados do Carrinho</returns>
    public Carrinho? Get(uint id)
    {
        return _context.Carrinhos
                    .FirstOrDefault
                    (a => a.Id == id);
    }

    /// <summary>
    /// Atualizar dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="Carrinho">novos dados do Carrinho</param>
    public void Edit(Carrinho carrinho)
    {
        ValidarCarrinho(carrinho);

        var existente = _context.Carrinhos.FirstOrDefault(c => c.Id == carrinho.Id);
        if (existente == null)
            throw new ServiceException("Carrinho não encontrado");

        existente.Observacao = carrinho.Observacao;
        existente.FormaDePagamento = carrinho.FormaDePagamento;
        existente.IdCliente = carrinho.IdCliente;
        existente.IdCartao = carrinho.IdCartao;
        existente.IdEndereco = carrinho.IdEndereco;
        existente.ValorFrete = carrinho.ValorFrete;

        _context.SaveChanges();

    }

    /// <summary>
    /// Remover dados de um Carrinho da base de dados
    /// </summary>
    /// <param name="id">id do Carrinho</param>
    public void Delete(uint id)
    {
        var Carrinho = _context.Carrinhos.FirstOrDefault(a => a.Id == id);
        if (Carrinho != null)
        {
            _context.Carrinhos.Remove(Carrinho);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Carrinho não encontrado");
        }
    }

    /// <summary>
    /// Obter dados de todos os Carrinhos da base de dados
    /// </summary>
    /// <returns>dados dos Carrinhos</returns>
    public IEnumerable<Carrinho> GetAll()
    {
        return _context.Carrinhos
            .AsNoTracking()
            .ToList();
    }

    // C#
    public void ValidarCarrinho(Carrinho carrinho)
    {
        if (carrinho.FormaDePagamento == "C" && carrinho.IdCartao == null)
            throw new ServiceException("Pagamento com cartão exige um cartão válido.");

        Cartao? cartao = null;
        if (carrinho.IdCartao != null)
        {
            cartao = _context.Cartaos
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == carrinho.IdCartao);
        }

        if (carrinho.FormaDePagamento == "C")
        {
            if (cartao == null)
                throw new ServiceException("Cartão não encontrado.");
            if (cartao.IdCliente != carrinho.IdCliente)
                throw new ServiceException("O cartão não pertence ao cliente selecionado.");
        }

        if (carrinho.FormaDePagamento != "C")
            carrinho.IdCartao = null;
    }


}





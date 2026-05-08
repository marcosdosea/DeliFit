using Core;
using Core.Service;

namespace Service;

public class AvaliacaoService : IAvaliacaoService
{
    private readonly DeliFitContext _context;

    public AvaliacaoService(DeliFitContext context)
    {
        _context = context;
    }

    public void Create(Avaliacao avaliacao)
    {
        _context.Avaliacaos.Add(avaliacao);
        _context.SaveChanges();
    }

    public void Edit(Avaliacao avaliacao)
    {
        _context.Avaliacaos.Update(avaliacao);
        _context.SaveChanges();
    }

    public void Delete(uint id)
    {
        var avaliacao = _context.Avaliacaos.Find(id);
        if (avaliacao != null)
        {
            _context.Avaliacaos.Remove(avaliacao);
            _context.SaveChanges();
        }
    }

    public Avaliacao? Get(uint id)
        => _context.Avaliacaos.Find(id);

    public IEnumerable<Avaliacao> GetAll()
        => _context.Avaliacaos.ToList();

    public IEnumerable<Avaliacao> GetByCliente(uint idCliente)
        => _context.Avaliacaos.Where(a => a.IdCliente == idCliente).ToList();

    public Avaliacao? GetByPedido(uint idPedido)
        => _context.Avaliacaos.FirstOrDefault(a => a.IdPedido == idPedido);
}
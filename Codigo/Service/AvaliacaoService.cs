using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
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

        public Avaliacao? Get(uint id)
        {
            return _context.Avaliacaos.Find(id);
        }

        public IEnumerable<Avaliacao> GetAll()
        {
            return _context.Avaliacaos.ToList();
        }

        public void Update(Avaliacao avaliacao)
        {
            _context.Entry(avaliacao).State = EntityState.Modified;
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

        public IEnumerable<Avaliacao> GetByPedido(uint idPedido)
        {
            return _context.Avaliacaos
                .Where(a => a.IdPedido == idPedido)
                .ToList();
        }

        public IEnumerable<Avaliacao> GetByCliente(uint idCliente)
        {
            return _context.Avaliacaos
                .Where(a => a.IdCliente == idCliente)
                .ToList();
        }
    }
}
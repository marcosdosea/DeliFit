using Core;
using Core.Service;

namespace Service
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly DeliFitContext _context;

        public AtendimentoService(DeliFitContext context)
        {
            _context = context;
        }

        public uint Create(Atendimento atendimento)
        {
            _context.Atendimentos.Add(atendimento);
            _context.SaveChanges();
            return atendimento.Id;
        }

        public void Delete(uint id)
        {
            var atendimento = _context.Atendimentos.Find(id);
            if (atendimento != null)
            {
                _context.Atendimentos.Remove(atendimento);
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Atendimento não encontrado");
            }
        }

        public void Edit(Atendimento atendimento)
        {
            _context.Atendimentos.Update(atendimento);
            _context.SaveChanges();
        }

        public Atendimento? Get(uint id)
        {
            return _context.Atendimentos.Find(id);
        }

        public IEnumerable<Atendimento> GetAll(uint idRestaurante)
        {
            return _context.Atendimentos.Where(a => a.IdRestaurante == idRestaurante)
                .ToList();
        }

    }
}

using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class RestauranteService : IRestauranteService
    {
        private readonly DeliFitContext _context;

        public RestauranteService(DeliFitContext context)
        {
            _context = context;
        }

        public uint Create(Restaurante restaurante)
        {
            _context.Add(restaurante);
            _context.SaveChanges();
            return restaurante.Id;
        }

        public void Delete(uint id)
        {
            var restaurante = _context.Restaurantes.FirstOrDefault(a => a.Id == id);
            if(restaurante != null)
            {
                _context.Remove(restaurante);
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Restaurante não encontrado");
            }
        }

        public void Edit(Restaurante restaurante)
        {
            var tracked = _context.ChangeTracker
            .Entries<Restaurante>()
            .FirstOrDefault(e => e.Entity.Id == restaurante.Id);

            if (tracked != null) tracked.State = EntityState.Detached;

            _context.Update(restaurante);
            _context.SaveChanges();
        }
        
        public Restaurante? Get(uint id)
        {
            return _context.Restaurantes
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Restaurante> GetAll()
        {
            return _context.Restaurantes
            .AsNoTracking()
            .OrderBy(a => a.Id);
        }
    }
}

using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    class RestauranteService : IRestauranteService
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
            if(Get(restaurante.Id) != null)
            {
                _context.Update(restaurante);
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Restaurante não encontrado");
            }
        }
        
        public Restaurante? Get(uint id)
        {
            return _context.Restaurantes.Find(id);
        }

        public IEnumerable<Restaurante> GetAll(int page, int pageSize)
        {
            return _context.Restaurantes
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }
    }
}

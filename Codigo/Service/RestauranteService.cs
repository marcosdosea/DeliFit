using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlX.XDevAPI;

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
            var restaurante = _context.Restaurantes.Find(id);
            if(restaurante != null)
            {
                _context.Entry(restaurante).Collection(c => c.Items).Load(); 
                _context.Items.RemoveRange(restaurante.Items); 
                _context.Restaurantes.Remove(restaurante); 
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Restaurante não encontrado");
            }
        }

        public void Edit(Restaurante restaurante)
        {
            _context.Update(restaurante);
            _context.SaveChanges();
        }
        
        public Restaurante? Get(uint id)
        {
            return _context.Restaurantes
                    .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<RestauranteDTO> GetAll()
        {
            return _context.Restaurantes
                .Select(r => new RestauranteDTO
                {
                    Id = r.Id,
                    NomeRestaurante = r.NomeRestaurante,
                    Validado= r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }
    }
}

using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlX.XDevAPI;

namespace Service
{
    /// <summary>
    /// Implementa os serviços para manter os dados de Restaurante.
    /// </summary>
    public class RestauranteService : IRestauranteService
    {
        private readonly DeliFitContext _context;
        
        public RestauranteService(DeliFitContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo restaurante na base de dados 
        /// </summary>
        /// <param name="restaurante">dados do editora</param>
        /// <returns>id gerado</returns>
        public uint Create(Restaurante restaurante)
        {
            _context.Add(restaurante);
            _context.SaveChanges();
            return restaurante.Id;
        }

        /// <summary>
        /// Remover restaurante da base de dados
        /// </summary>
        /// <param name="id">id a ser removido</param>
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

        /// <summary>
        /// Atualizar dados do restaurante
        /// </summary>
        /// <param name="restaurante">novos dados do restaurante</param>
        public void Edit(Restaurante restaurante)
        {
            _context.Update(restaurante);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obter os dados de um restaurante na base de dados
        /// </summary>
        /// <param name="id">id do restaurante</param>
        /// <returns>Dados do restaurante</returns>
        public Restaurante? Get(uint id)
        {
            return _context.Restaurantes
                    .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Obter dados de todos os restaurantes na base de dados
        /// </summary>
        /// <returns>lista de restaurantes</returns>
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

        public IEnumerable<FaturamentoDTO> GetAllFaturamentos(uint idRestaurante)
        {
           var faturamentos = _context.Pedidos
                .Where(p => p.IdRestaurante == idRestaurante && p.Data.HasValue)
                .GroupBy(p => p.Data.Value.Date)
                .Select(g => new FaturamentoDTO
                {
                    IdRestaurante = idRestaurante,
                    Data = g.Key,
                    TotalFaturamento = g.Sum(p => p.Preco),
                    TotalPedidos = g.Count(),
                    Pedidos = g.ToList()
                })
                .ToList();
            return faturamentos;
        }
    }
}

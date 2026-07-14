using Core;
using Core.DTO;
using Core.Service;

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
            if (restaurante != null)
            {
                // Remove Atendimentos relacionados
                _context.Entry(restaurante).Collection(c => c.Atendimentos).Load();
                _context.Atendimentos.RemoveRange(restaurante.Atendimentos);

                // Remove Itens relacionados
                _context.Entry(restaurante).Collection(c => c.Items).Load();
                _context.Items.RemoveRange(restaurante.Items);

                // Remove Pagamentos relacionados
                _context.Entry(restaurante).Collection(c => c.Pagamentos).Load();
                _context.Pagamentos.RemoveRange(restaurante.Pagamentos);

                // Remove Pedidos relacionados
                _context.Entry(restaurante).Collection(c => c.Pedidos).Load();
                _context.Pedidos.RemoveRange(restaurante.Pedidos);

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
                    Validado = r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }

        public IEnumerable<RestauranteDTO> GetRestaurantesAtivos()
        {
            return _context.Restaurantes
                .Where(r => r.Validado == true)
                .Select(r => new RestauranteDTO
                {
                    Id = r.Id,
                    NomeRestaurante = r.NomeRestaurante,
                    Validado = r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }

        public IEnumerable<RestauranteDTO> GetRestaurantesPendentes()
        {
            return _context.Restaurantes
                .Where(r => r.Validado == false)
                .Select(r => new RestauranteDTO
                {
                    Id = r.Id,
                    NomeRestaurante = r.NomeRestaurante,
                    Validado = r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }

        public IEnumerable<FaturamentoDTO> GetAllFaturamentos(uint idRestaurante, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _context.Pedidos
                .Where(p => p.IdRestaurante == idRestaurante && p.Data.HasValue);

            if (dataInicio.HasValue)
                query = query.Where(p => p.Data.Value.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                query = query.Where(p => p.Data.Value.Date <= dataFim.Value.Date);

            return query
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
        }

        public Restaurante? GetByEmail(string email)
        {
            return _context.Restaurantes
                .FirstOrDefault(r => r.Email == email);
        }

        /// <summary>
        /// Busca restaurantes ativos cujo nome contenha o termo (case-insensitive).
        /// </summary>
        public IEnumerable<RestauranteDTO> Buscar(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return GetRestaurantesAtivos();

            var termoLower = termo.ToLower();
            return _context.Restaurantes
                .Where(r => r.Validado && r.NomeRestaurante.ToLower().Contains(termoLower))
                .Select(r => new RestauranteDTO
                {
                    Id = r.Id,
                    NomeRestaurante = r.NomeRestaurante,
                    Validado = r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }

        /// <summary>
        /// Retorna restaurantes ativos que possuem ao menos um item com a restrição indicada.
        /// </summary>
        public IEnumerable<RestauranteDTO> GetByRestricao(string restricao)
        {
            var idsComRestricao = _context.Items
                .Where(i => i.Restricao == restricao)
                .Select(i => i.IdRestaurante)
                .Distinct();

            return _context.Restaurantes
                .Where(r => r.Validado && idsComRestricao.Contains(r.Id))
                .Select(r => new RestauranteDTO
                {
                    Id = r.Id,
                    NomeRestaurante = r.NomeRestaurante,
                    Validado = r.Validado,
                    Cidade = r.Cidade,
                    Estado = r.Estado
                })
                .ToList();
        }
    }
}

using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;

namespace Service
{

    /// <summary>
    /// Implementa os serviços para manter os dados de clientes
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly DeliFitContext _context;

        public ClienteService(DeliFitContext context)
        {
            _context = context;

        }

        /// <summary>
        /// Cria um novo cliente na base de dados
        /// </summary>
        /// <param name="cliente">dados do cliente</param>
        /// <returns>id do novo cliente</returns>
        public uint Create(Cliente cliente)
        {
            _context.Add(cliente);
            _context.SaveChanges();
            return cliente.Id;
        }

        /// <summary>
        /// Obter os dados de um cliente da base de dados
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns>dados do cliente</returns>
        public Cliente? Get(uint id)
        {
            return _context.Clientes
                        .FirstOrDefault
                        (a => a.Id == id);
        }

        /// <summary>
        /// Atualizar dados de um cliente da base de dados
        /// </summary>
        /// <param name="cliente">novos dados do cliente</param>
        public void Edit(Cliente cliente)
        {

            _context.Update(cliente);
            _context.SaveChanges();

        }

        /// <summary>
        /// Remover dados de um cliente da base de dados
        /// </summary>
        /// <param name="id">id do cliente</param>
        public void Delete(uint id)
        {
            var Cliente = _context.Clientes.FirstOrDefault(a => a.Id == id);
            if (Cliente != null)
            {
                _context.Entry(Cliente).Collection(c => c.Enderecos).Load();
                _context.Enderecos.RemoveRange(Cliente.Enderecos);
                _context.Clientes.Remove(Cliente);
                _context.SaveChanges();
            }
            else
            {
                throw new ServiceException("Cliente não encontrado");
            }
        }

        /// <summary>
        /// Obter dados de todos os clientes da base de dados
        /// </summary>
        /// <returns>dados dos clientes</returns>
        public IEnumerable<ClienteDTO> GetAll()
        {
            return _context.Clientes
                .Select(r => new ClienteDTO
                {
                    Id = r.Id,
                    Nome = r.Nome,
                    Telefone = r.Telefone,
                    Email = r.Email
                })
                .ToList();
        }

    }
}

     



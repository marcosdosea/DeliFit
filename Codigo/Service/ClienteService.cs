using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

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

    public uint Create(Cliente cliente)
    {
        _context.Add(cliente);
        _context.SaveChanges();
        return cliente.Id;
    }

    public Cliente? Get(uint id)
    {
        return _context.Clientes
            .FirstOrDefault(a => a.Id == id);
    }

    public void Edit(Cliente cliente)
    {
        _context.Update(cliente);
        _context.SaveChanges();
    }

    public void Delete(uint id)
    {
        var cliente = _context.Clientes.FirstOrDefault(a => a.Id == id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
        }
        else
        {
            throw new ServiceException("Cliente não encontrado");
        }
    }

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

    /// <summary>
    /// Busca um cliente pelo e-mail para autenticação
    /// </summary>
    public Cliente? GetByEmail(string email)
    {
        return _context.Clientes
            .FirstOrDefault(c => c.Email == email);
    }

    /// <summary>
    /// Busca um cliente pelo telefone para autenticação
    /// </summary>
    public Cliente? GetByTelefone(string telefone)
    {
        return _context.Clientes
            .FirstOrDefault(c => c.Telefone == telefone);
    }

    /// <summary>
    /// Busca um cliente pelo CPF de forma assíncrona
    /// </summary>
    public async Task<Cliente?> GetByCpf(string cpf)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Cpf == cpf);
    }
}
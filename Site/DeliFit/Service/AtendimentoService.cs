using Core;
using Core.Service;

namespace Service;

/// <summary>
/// Implementa os serviços para manter os horários de atendimento de um Restaurante.
/// </summary>
public class AtendimentoService : IAtendimentoService
{
    private readonly DeliFitContext _context;

    public AtendimentoService(DeliFitContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Criar um novo horário atendimento na base de dados 
    /// </summary>
    /// <param name="atendimento">dados do horário</param>
    /// <returns>id gerado</returns>
    public uint Create(Atendimento atendimento)
    {
        ValidarAtendimento(atendimento);
        _context.Atendimentos.Add(atendimento);
        _context.SaveChanges();
        return atendimento.Id;
    }

    /// <summary>
    /// Remover horário de atendimento da base de dados
    /// </summary>
    /// <param name="id">id a ser removido</param>
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

    /// <summary>
    /// Atualizar dados de um horário de atendimento na base de dados
    /// </summary>
    /// <param name="atendimento">novos dados do horário de atendimento</param>
    public void Edit(Atendimento atendimento)
    {
        ValidarAtendimento(atendimento, atendimento.Id);
        _context.Atendimentos.Update(atendimento);
        _context.SaveChanges();
    }

    /// <summary>
    /// Obter os dados de um atendimento de um restaurante na base de dados
    /// </summary>
    /// <param name="id">id do horário de atendimento</param>
    /// <returns>Dados do horário de atendimento</returns>
    public Atendimento? Get(uint id)
    {
        return _context.Atendimentos.Find(id);
    }

    /// <summary>
    /// Obter uma lista com todos os horários de atendimento de um restaurante
    /// </summary>
    /// <returns>lista de horários de atendimento</returns>
    public IEnumerable<Atendimento> GetAll(uint idRestaurante)
    {
        return _context.Atendimentos.Where(a => a.IdRestaurante == idRestaurante).ToList();
    }

    private void ValidarAtendimento(Atendimento atendimento, uint idExcluido = 0)
    {
        if (atendimento.IdRestaurante == 0)
            throw new ServiceException("Restaurante inválido.");

        if (atendimento.HorarioInicio.HasValue && atendimento.HorarioFim.HasValue
            && atendimento.HorarioInicio >= atendimento.HorarioFim)
            throw new ServiceException("O horário de início deve ser anterior ao horário de fim.");

        var existeDiaDuplicado = _context.Atendimentos.Any(a =>
            a.IdRestaurante == atendimento.IdRestaurante
            && a.DiaSemana == atendimento.DiaSemana
            && a.Id != idExcluido);

        if (existeDiaDuplicado)
            throw new ServiceException("Já existe um horário configurado para este dia da semana neste restaurante.");
    }
}

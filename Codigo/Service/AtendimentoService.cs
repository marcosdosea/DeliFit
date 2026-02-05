using Core;
using Core.Service;

namespace Service
{
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
            IEnumerable<Atendimento> listaAtendimentos = _context.Atendimentos.Where(a => a.IdRestaurante == idRestaurante);

            foreach (Atendimento atendimento in listaAtendimentos)
            {
                switch(atendimento.DiaSemana)
                {
                    case "1":
                        atendimento.DiaSemana = "Domingo";
                        break;
                    case "2":
                        atendimento.DiaSemana = "Segunda-feira";
                        break;
                    case "3":
                        atendimento.DiaSemana = "Terça-feira";
                        break;
                    case "4":
                        atendimento.DiaSemana = "Quarta-feira";
                        break;
                    case "5":
                        atendimento.DiaSemana = "Quinta-feira";
                        break;
                    case "6":
                        atendimento.DiaSemana = "Sexta-feira";
                        break;
                    case "7":
                        atendimento.DiaSemana = "Sábado";
                        break;
                }
            }

            return listaAtendimentos;
        }

    }
}

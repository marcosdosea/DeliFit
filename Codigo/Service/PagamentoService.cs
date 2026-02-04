using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{

    /// <summary>
    /// Implementa os serviços para manter os dados do pagamento das mensalidades
    /// </summary>
    public class PagamentoService : IPagamentoService
    {
        private readonly DeliFitContext _context;
        public PagamentoService(DeliFitContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Cria um novo registro de pagamento na base de dados
        /// </summary>
        /// <param name="pagamento">dados do pagamento</param>
        /// <returns>id do novo pagamento</returns>
        public uint Create(Pagamento pagamento)
        {
            _context.Add(pagamento);
            _context.SaveChanges();
            return pagamento.Id;
        }

        /// <summary>
        /// Obter os dados de um pagamento da base de dados
        /// </summary>
        /// <param name="id">id do pagamento</param>
        /// <returns>dados do pagamento</returns>
        public Pagamento? Get(uint id)
        {
            return _context.Pagamentos
                        .FirstOrDefault
                        (a => a.Id == id);
        }


        /// <summary>
        /// Obter dados de todos os pagamentos da base de dados
        /// </summary>
        /// <returns>dados dos pagamentos</returns>
        public IEnumerable<Pagamento> GetAll()
        {
            return _context.Pagamentos.AsNoTracking();
        }


        /// <summary>
        /// Obter dados de todos os pagamentos de um restaurante específico
        /// </summary>
        /// <param name="idRestaurante"></param>
        /// <returns>dados dos pagamentos referente a um determinado restaurante</returns>
        public IEnumerable<Pagamento> GetAllByRestaurante(uint idRestaurante)
        {
            return _context.Pagamentos
                .AsNoTracking()
                .Where(p => p.IdRestaurante == idRestaurante);
        }
        

        /// <summary>
        /// Obter dados de todos os pagamentos com base em um status específico
        /// </summary>
        /// <param name="statusMensalidade"></param>
        /// <returns>dados dos pagamentos referente a um status das mensalidades</returns>
        public IEnumerable<Pagamento> GetAllByStatus(string statusMensalidade)
        {
            return _context.Pagamentos
                .AsNoTracking()
                .Where(p => p.StatusMensalidade == statusMensalidade);
        }
    }
}

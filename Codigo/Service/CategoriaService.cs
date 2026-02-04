using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    /// <summary>
    /// Implementar os serviços para manter os dados de categorias
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly DeliFitContext _context;

        public CategoriaService(DeliFitContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Obter a lista das categorias disponiveis no sistema
        /// </summary>
        /// <returns>retorna todas as categorias registradas</returns>
        public IEnumerable<CategoriaDTO> ListarCategorias()
        {
            return _context.Items
               .GroupBy(i => i.Restricao)
               .Select(g => new CategoriaDTO
               {
                   Nome = g.Key, //Nome da categoria ficou armazenado por conta do groupby em g.key
                   QuantidadeItens = g.Count()
               })
               .ToList();
        }


        /// <summary>
        /// Obter a lista de itens por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns>retorna todos os itens da categoria especificada</returns>
        public IEnumerable<Item> ListarItensPorCategoria(string categoria)
        {
            
            return _context.Items
                .Where(i => i.Restricao == categoria)
                .ToList();
        }

        
    }
}

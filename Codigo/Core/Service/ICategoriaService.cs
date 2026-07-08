using Core.DTO;

namespace Core.Service
{
    public interface ICategoriaService
    {
        IEnumerable<CategoriaDTO> ListarCategorias();
        IEnumerable<Item> ListarItensPorCategoria(string categoria);
        void Create(string nome);
        void Delete(uint id);
        Categoria? Get(uint id);
    }
}

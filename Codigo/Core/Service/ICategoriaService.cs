using Core.DTO;

namespace Core.Service
{
    public interface ICategoriaService
    {
        IEnumerable<CategoriaDTO> ListarCategorias();

        IEnumerable<Item> ListarItensPorCategoria(string categoria);

        IEnumerable<Categoria> ListarTodas();
    }
}

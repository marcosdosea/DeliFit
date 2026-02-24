using Core.DTO;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IBrasilApiService
    {
        // Retorna true se o DDD for válido, falso caso contrário
        Task<bool> IsDddValidAsync(string ddd);

        // Opcional: Retorna os dados completos do DDD (estado e cidades)
        Task<DddResponse?> GetDddInfoAsync(string ddd);
    }
}
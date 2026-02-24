using Core.DTO;
using Core.Service;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class BrasilApiService : IBrasilApiService
    {
        private readonly HttpClient _httpClient;

        // O HttpClient será injetado automaticamente pelo .NET
        public BrasilApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsDddValidAsync(string ddd)
        {
            //if (string.IsNullOrWhiteSpace(ddd) || ddd.Length != 2) return false;

            //var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/ddd/v1/{ddd}");

            //// Se retornar 200 OK, o DDD existe. Se retornar 404, não existe.
            //return response.IsSuccessStatusCode;

            if (string.IsNullOrWhiteSpace(ddd)) return false;
            // mantém só dígitos e remove espaços/caracteres
            ddd = new string(ddd.Where(char.IsDigit).ToArray());
            if (ddd.Length != 2) return false;

            try
            {
                var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/ddd/v1/{ddd}");
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                // log opcional
                return false;
            }

        }

        public async Task<DddResponse?> GetDddInfoAsync(string ddd)
        {
            if (string.IsNullOrWhiteSpace(ddd)) 
                return null;
            // mantém só dígitos e remove espaços/caracteres
            ddd = new string(ddd.Where(char.IsDigit).ToArray());
            if (ddd.Length != 2) 
                return null;

            var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/ddd/v1/{ddd}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DddResponse>(jsonString);
            }

            return null; // Retorna nulo se o DDD for inválido
        }
    }
}
using System.Text.Json.Serialization;

namespace DeliFitWeb.Models;

public class CnpjResponse
{
    [JsonPropertyName("razao_social")]
    public string razao_social { get; set; }

    [JsonPropertyName("nome_fantasia")]
    public string nome_fantasia { get; set; }

    [JsonPropertyName("logradouro")]
    public string logradouro { get; set; }

    [JsonPropertyName("numero")]
    public string numero { get; set; }

    [JsonPropertyName("bairro")]
    public string bairro { get; set; }

    [JsonPropertyName("municipio")]
    public string municipio { get; set; }

    [JsonPropertyName("uf")]
    public string uf { get; set; }

    [JsonPropertyName("cep")]
    public string cep { get; set; }
}
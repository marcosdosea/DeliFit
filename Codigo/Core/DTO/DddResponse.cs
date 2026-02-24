using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.DTO
{
    public class DddResponse
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("cities")]
        public List<string> Cities { get; set; } = new();
    }
}
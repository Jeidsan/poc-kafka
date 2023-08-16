using System.Text.Json.Serialization;

namespace Kafka.Model
{
    public class Pessoa
    {
        [JsonPropertyName("Nome")]
        public string? Nome { get; set; }
        [JsonPropertyName("Idade")]
        public int Idade { get; set; }
    }
}
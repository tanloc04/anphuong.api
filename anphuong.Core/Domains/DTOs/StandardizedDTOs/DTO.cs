using System.Text.Json.Serialization;

namespace anphuong.Core.Domains.DTOs.StandardizedDTOs
{
    public class DTO
    {
        [JsonPropertyOrder(-100)]
        public string Id { get; set; }

        [JsonPropertyName("createdAt")]
        [JsonPropertyOrder(100)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        [JsonPropertyOrder(200)]
        public DateTime UpdatedAt { get; set; }
    }
}

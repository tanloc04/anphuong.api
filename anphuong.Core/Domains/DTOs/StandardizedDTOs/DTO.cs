using System.Text.Json.Serialization;

namespace anphuong.Core.Domains.DTOs.StandardizedDTOs
{
    public class DTO
    {
        [JsonPropertyOrder(-100)]
        public int Id { get; set; }

        [JsonPropertyName("createdAt")]
        [JsonPropertyOrder(100)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        [JsonPropertyOrder(101)]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("isDeleted")]
        [JsonPropertyOrder(102)]
        public bool IsDeleted { get; set; }

    }
}

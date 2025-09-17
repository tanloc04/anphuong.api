using System.Text.Json.Serialization;

namespace anphuong.Core.Domains.DTOs
{
    public class CustomerUserDTO
    {
        public int Id { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}

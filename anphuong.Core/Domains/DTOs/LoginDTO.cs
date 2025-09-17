using System.Text.Json.Serialization;

namespace anphuong.Core.Domains.DTOs
{
    public class LoginDTO
    {
        [JsonPropertyName("token")]
        public string? AccessToken { get; set; }
    }
}

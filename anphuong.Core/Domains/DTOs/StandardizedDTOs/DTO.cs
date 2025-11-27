using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

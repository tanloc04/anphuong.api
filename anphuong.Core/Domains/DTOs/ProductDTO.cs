using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; 
        }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int LongSize { get; set; }
        public int WidthSize { get; set; }
        public int HeightSize { get; set; }
        public string Material { get; set; }
        public int DetailImageId { get; set; }
        public int CategoryId { get; set; }
        public int VariationId { get; set; }
    }
}

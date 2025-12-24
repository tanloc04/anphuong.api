using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Domains.DTOs
{
    public class VariantDTO : DTO
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public Color Color { get; set; }
        public ProductDTO Product { get; set; }
    }
}

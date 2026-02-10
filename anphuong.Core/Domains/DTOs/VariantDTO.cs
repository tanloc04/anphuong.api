using anphuong.Core.Domains.DTOs.ResponseDTOs.Color;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Domains.DTOs
{
    public class VariantDTO : DTO
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public int VariantImageId { get; set; }
        public string? VariantImage { get; set; }
        public ColorNameDTO Color { get; set; }
        //public ProductDTO Product { get; set; }
    }
}

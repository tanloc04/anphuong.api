using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.ResponseDTOs.Product
{
    public class ProductListResponseDTO : DTO
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int LongSize { get; set; }
        public int WidthSize { get; set; }
        public int HeightSize { get; set; }
        public string Material { get; set; }
        public string Thumbnail { get; set; }
        public int CategoryId { get; set; }
        public int? VariationId { get; set; }
    }
}

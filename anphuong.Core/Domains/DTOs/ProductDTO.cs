using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class ProductDTO : DTO
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal LongSize { get; set; }
        public decimal WidthSize { get; set; }
        public decimal HeightSize { get; set; }
        public bool isCustomize { get; set; }
        public int? CategoryId { get; set; }
        public ProductDetailImageDTO DetailImage { get; set; }
        public ProductCategoryDTO Category { get; set; }
        public int TotalStock { get; set; }
        public bool IsMissingVariants { get; set; }
        public string? Thumbnail { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public ICollection<VariantDTO> Variants { get; set; }
    }
}

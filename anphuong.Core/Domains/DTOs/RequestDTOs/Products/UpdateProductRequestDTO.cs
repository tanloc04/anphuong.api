using anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Products
{
    public class UpdateProductRequestDTO
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string? Description { get; set; }
        public int? LongSize { get; set; }
        public int? WidthSize { get; set; }
        public int? HeightSize { get; set; }
        public string? Material { get; set; }
        public UpdateDetailImageRequestDTO? DetailImage { get; set; }
        public int? DetailImageId { get; set; }
        public int? CategoryId { get; set; }
        public int? VariationId { get; set; }
        public int? Stock { get; set; }

    }
}

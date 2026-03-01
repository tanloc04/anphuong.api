using System.ComponentModel;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Products
{
    public class CreateProductRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        public decimal LongSize { get; set; }
        public decimal WidthSize { get; set; }
        public decimal HeightSize { get; set; }

        [DefaultValue(false)]
        public bool isCustomize { get; set; } = false;
        public int CategoryId { get; set; }
        public string Image1 { get; set; } = string.Empty;
        public string Image2 { get; set; } = string.Empty;
        public string Image3 { get; set; } = string.Empty;
        public string Image4 { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}

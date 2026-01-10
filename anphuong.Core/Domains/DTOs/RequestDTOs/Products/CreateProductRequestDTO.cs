namespace anphuong.Core.Domains.DTOs.RequestDTOs.Products
{
    public class CreateProductRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Material { get; set; }
        public int LongSize { get; set; }
        public int WidthSize { get; set; }
        public int HeightSize { get; set; }
        public int CategoryId { get; set; }
        public string? Thumbnail { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public int Stock { get; set; }
    }
}

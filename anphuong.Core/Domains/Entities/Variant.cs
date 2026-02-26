namespace anphuong.Core.Domains.Entities
{
    public class Variant : Entity
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        public string VariantImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Color Color { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Material Material { get; set; } = null!;
    }
}

namespace anphuong.Core.Domains.Entities
{
    public class Variant : Entity
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public string VariantImage { get; set; }
        public Color Color { get; set; }
        public Product Product { get; set; }
    }
}

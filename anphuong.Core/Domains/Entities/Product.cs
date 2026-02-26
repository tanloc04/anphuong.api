namespace anphuong.Core.Domains.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal LongSize { get; set; }
        public decimal WidthSize { get; set; }
        public decimal HeightSize { get; set; }
        public bool isCustomize { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Variant> Variants { get; set; }
        public ICollection<Behavior> Behaviors { get; set; }
    }
}

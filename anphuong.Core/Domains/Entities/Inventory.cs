namespace anphuong.Core.Domains.Entities
{
    public class Inventory : Entity
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public Variant Variant { get; set; }
    }
}

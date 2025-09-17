namespace anphuong.Core.Domains.Entities
{
    public class Inventory : Entity
    {
        public int QuantityInStock { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

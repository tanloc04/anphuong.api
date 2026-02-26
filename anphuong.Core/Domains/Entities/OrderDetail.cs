namespace anphuong.Core.Domains.Entities
{
    public class OrderDetail : Entity
    {   
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double SubTotal { get; set; }
        public int VariantId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public Variant Variant { get; set; }
    }
}

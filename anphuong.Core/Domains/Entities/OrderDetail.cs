namespace anphuong.Core.Domains.Entities
{
    public class OrderDetail : Entity
    {   
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double SubTotal { get; set; }
        public bool isCustomized { get; set; } = false;
        public decimal? CustomLongSize { get; set; }
        public decimal? CustomWidthSize { get; set; }
        public decimal? CustomHeightSize { get; set; }
        public int VariantId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public Variant Variant { get; set; }
    }
}

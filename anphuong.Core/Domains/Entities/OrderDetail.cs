namespace anphuong.Core.Domains.Entities
{
    public class OrderDetail : Entity
    {
        public bool IsCustomize { get; set; }
        public int CustomizeHeight { get; set; }
        public int CustomizeWidth { get; set; }
        public int CustomizeLong { get; set; }
        public string CustomizeMaterial { get; set; }
        public int Quantity { get; set; }
        public double SubTotalPrice { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}

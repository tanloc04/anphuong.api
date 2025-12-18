namespace anphuong.Core.Domains.DTOs.RequestDTOs.Orders
{
    public class ProductPriceSnapshot
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }
}

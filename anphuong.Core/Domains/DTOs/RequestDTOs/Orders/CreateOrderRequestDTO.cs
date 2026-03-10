namespace anphuong.Core.Domains.DTOs.RequestDTOs.Orders
{
    public class CreateOrderRequestDTO
    {
        public bool IsNewCustomer { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public DateTime? DeliveryDate { get; set; }
        public int PaymentMethod { get; set; }
        public double TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime ShippingDate { get; set; }      
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }
}

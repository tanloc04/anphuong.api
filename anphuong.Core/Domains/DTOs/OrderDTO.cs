using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class OrderDTO : DTO
    {
        public int Id { get; set; }
        public int PaymentMethod { get; set; }
        public int Status { get; set; }
        public DateTime? ShippingDate { get; set; }
        public double TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public CustomerDTO Customer { get; set; } = null!;
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();

        public string? ShippingAddress { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
    }
}

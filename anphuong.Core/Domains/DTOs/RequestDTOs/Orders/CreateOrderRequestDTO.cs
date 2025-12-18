namespace anphuong.Core.Domains.DTOs.RequestDTOs.Orders
{
    public class CreateOrderRequestDTO
    {
        public int PaymentMethod { get; set; }
        public int Status { get; set; }
        public DateTime ShippingDate { get; set; }
        public int CustomerId { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }
}

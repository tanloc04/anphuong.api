namespace anphuong.Core.Domains.DTOs.RequestDTOs.Order
{
    public class UpdateOrderRequestDTO
    {
        public int? PaymentMethod { get; set; }
        public int? Status { get; set; }
        public DateTime? ShippingDate { get; set; }
        public double? TotalPrice { get; set; }
    }
}

namespace anphuong.Core.Domains.Entities
{
    public class Order : Entity
    {
        public int PaymentMethod { get; set; } //0: COD, 1: Banking, 2: Offline
        public int Status { get; set; } //0: cancel, 1: processing(default), 2: delivering, 3: delivered, 4: refund
        public DateTime ShippingDate { get; set; }
        public double TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

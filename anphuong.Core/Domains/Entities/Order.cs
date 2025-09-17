namespace anphuong.Core.Domains.Entities;

public partial class Order : Entity
{
    public int? PaymentMethod { get; set; }

    public int? Status { get; set; }

    public DateTime? ShippingDate { get; set; }

    public double? TotalPrice { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

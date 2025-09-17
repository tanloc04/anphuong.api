namespace anphuong.Core.Domains.Entities;

public partial class OrderDetail : Entity
{
    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public bool? IsCustomize { get; set; }

    public int? CustomizeHeight { get; set; }

    public int? CustomizeWidth { get; set; }

    public int? CustomizeLong { get; set; }

    public string? CustomizeMaterial { get; set; }

    public int Quantity { get; set; }

    public double? Subtotal { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

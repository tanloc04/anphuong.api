namespace anphuong.Core.Domains.Entities;

public partial class Product : Entity
{
    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public double? Discount { get; set; }

    public string? Description { get; set; }

    public int? LongSize { get; set; }

    public int? HeightSize { get; set; }

    public int? WidthSize { get; set; }

    public int? DetailImageId { get; set; }

    public string? Material { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<Behavior> Behaviors { get; set; } = new List<Behavior>();

    public virtual Category Category { get; set; } = null!;

    public virtual DetailImage? DetailImage { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}

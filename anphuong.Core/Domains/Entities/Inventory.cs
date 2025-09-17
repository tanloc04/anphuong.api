namespace anphuong.Core.Domains.Entities;

public partial class Inventory : Entity
{
    public int ProductId { get; set; }

    public int? QuantityInStock { get; set; }

    public virtual Product Product { get; set; } = null!;
}

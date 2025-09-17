namespace anphuong.Core.Domains.Entities;

public partial class Variant : Entity
{
    public int ColorId { get; set; }

    public int ProductId { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

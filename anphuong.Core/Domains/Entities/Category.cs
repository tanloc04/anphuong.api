namespace anphuong.Core.Domains.Entities;

public partial class Category : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

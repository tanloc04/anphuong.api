namespace anphuong.Core.Domains.Entities;

public partial class Color : Entity
{
    public string? ColorName { get; set; }

    public string? HexCode { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}

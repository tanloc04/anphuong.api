namespace anphuong.Core.Domains.Entities;

public partial class DetailImage : Entity
{
    public string? ImageStringThumbnail { get; set; }

    public string? ImageString01 { get; set; }

    public string? ImageString02 { get; set; }

    public string? ImageString03 { get; set; }

    public string? ImageString04 { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

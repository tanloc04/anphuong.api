namespace anphuong.Core.Domains.Entities;

public partial class Behavior : Entity
{

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int? ViewCount { get; set; }

    public int? Buy { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

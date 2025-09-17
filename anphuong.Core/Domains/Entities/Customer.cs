namespace anphuong.Core.Domains.Entities;

public partial class Customer : Entity
{
    public string Fullname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? CustomerAddress { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Behavior> Behaviors { get; set; } = new List<Behavior>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}

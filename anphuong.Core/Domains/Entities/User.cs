namespace anphuong.Core.Domains.Entities;

public partial class User : Entity
{

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Status { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}

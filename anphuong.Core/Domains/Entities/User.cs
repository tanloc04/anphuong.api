namespace anphuong.Core.Domains.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; }
        public Customer Customer { get; set; }
    }
}

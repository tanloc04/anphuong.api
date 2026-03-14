using Org.BouncyCastle.Bcpg.OpenPgp;

namespace anphuong.Core.Domains.Entities
{
    public class User : Entity
    {
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public bool Role { get; set; } //0: User, 1: Admin
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }
        public virtual Customer Customer { get; set; }
    }
}

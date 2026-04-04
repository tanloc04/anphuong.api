namespace anphuong.Core.Domains.DTOs.RequestDTOs.Auth
{
    public class GoogleRegisterRequestDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}

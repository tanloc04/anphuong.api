using System.Text.Json.Serialization;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class CustomerUserDTO : DTO
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}

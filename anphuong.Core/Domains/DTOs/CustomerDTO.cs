using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class CustomerDTO : DTO
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? CustomerAddress { get; set; }

    }
}

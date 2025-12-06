using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Auth
{
    public class GoogleRegisterRequestDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.AuthController
{
    public class GoogleLoginRequestDTO
    {
        public string IdToken { get; set; } = null!;
    }
}

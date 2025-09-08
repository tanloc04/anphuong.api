using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.API
{
    public class ValidationErrorDTO
    {
        public string? Field { get; set; }
        public List<string>? Message { get; set; }
    }
}

using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs
{
    public class ColorDTO : DTO
    {
        public string Name { get; set; }
        public string HexCode { get; set; }
    }
}

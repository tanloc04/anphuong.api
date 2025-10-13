using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class DetailImageDTO : DTO
    {
        public string Thumbnail { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
    }
}

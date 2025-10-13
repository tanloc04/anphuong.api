using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage
{
    public class CreateDetailImageRequestDTO
    {
        public IFormFile Thumbnail { get; set; }
        public IFormFile Image1 { get; set; }
        public IFormFile Image2 { get; set; }
        public IFormFile Image3 { get; set; }
        public IFormFile Image4 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.ResponseDTOs.Product
{
    public class ProductDetailImageDTO
    {
        public int Id { get; set; }
        public string Thumbnail { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
    }
}

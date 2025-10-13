using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Product
{
    public class UpdateProductRequestDTO
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string? Description { get; set; }
        public int? LongSize { get; set; }
        public int? WidthSize { get; set; }
        public int? HeightSize { get; set; }
        public string? Material { get; set; }
        public IFormFile Thumnail { get; set; }
        public IFormFile? Image1 { get; set; }
        public IFormFile? Image2 { get; set; }
        public IFormFile? Image3 { get; set; }
        public IFormFile? Image4 { get; set; }
        public int? CategoryId { get; set; }
        public int? VariationId { get; set; }
    }
}

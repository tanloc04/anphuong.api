using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Product
{
    public class CreateProductImageRequestDTO
    {
        public CreateProductRequestDTO ProductInfo { get; set; } = null!;
        public CreateDetailImageRequestDTO Images { get; set; } = null!;
    }
}

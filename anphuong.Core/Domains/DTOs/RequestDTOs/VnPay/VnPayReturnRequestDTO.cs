using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.VnPay
{
    public class VnPayReturnRequestDTO
    {
        public int OrderId { get; set; }
        public string ResponseCode { get; set; }
    }
}

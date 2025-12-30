using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Customer
{
    public class UpdateCustomerRequestDTO
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? CustomerAddress { get; set; }
    }
}

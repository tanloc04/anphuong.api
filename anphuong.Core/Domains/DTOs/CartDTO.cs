using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs
{
    public class CartDTO : DTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public int TotalItems => CartItems?.Sum(x => x.Quantity) ?? 0;
        public decimal TotalPrice => CartItems?.Sum(x => x.Quantity * x.Price) ?? 0;

    }
}

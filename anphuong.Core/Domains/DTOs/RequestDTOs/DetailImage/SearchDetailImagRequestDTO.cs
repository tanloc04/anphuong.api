using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage
{
    public class SearchDetailImageRequestDTO : SearchRequestDTO
    {
        public new SearchDetailImageCondition SearchCondition { get; set; } = new();
    }

    public class SearchDetailImageCondition
    {
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

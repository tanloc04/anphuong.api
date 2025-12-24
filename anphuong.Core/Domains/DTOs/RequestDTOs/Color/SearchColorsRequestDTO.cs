using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Color
{
    public class SearchColorsCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class SearchColorsRequestDTO : SearchRequestDTO<SearchColorsCondition>
    {
        
    }


}

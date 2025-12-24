using anphuong.Core.Domains.DTOs.StandardizedDTOs;

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

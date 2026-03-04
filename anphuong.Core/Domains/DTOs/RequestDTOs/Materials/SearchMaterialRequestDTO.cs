using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Materials
{
    public class SearchMaterialsCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class SearchMaterialRequestDTO : SearchRequestDTO<SearchMaterialsCondition>
    {
            
    }
}

using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Product
{
    public class SearchProductCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SearchProductsRequestDTO
        : SearchRequestDTO<SearchProductCondition>
    {
    }
}

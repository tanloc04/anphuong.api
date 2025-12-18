using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Category
{
    public class SearchCategoriesCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SearchCategoriesRequestDTO
        : SearchRequestDTO<SearchCategoriesCondition>
    {
    }
}

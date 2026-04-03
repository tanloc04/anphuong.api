using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Products
{
    public class SearchProductCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }

    public class SearchProductsRequestDTO
        : SearchRequestDTO<SearchProductCondition>
    {
    }
}

using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Orders
{
    public class SearchOrderCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class SearchOrderRequestDTO
        : SearchRequestDTO<SearchOrderCondition>
    {
    }
}

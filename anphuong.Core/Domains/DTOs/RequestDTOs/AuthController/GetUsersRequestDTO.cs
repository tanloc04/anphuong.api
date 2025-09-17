using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.AuthController
{
    public class GetUsersRequestDTO
    {
        public SearchCondition? SearchCondition { get; set; } = null!;

        public PageInfoRequestDTO? PageInfo { get; set; } = null!;
    }
    public class SearchCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

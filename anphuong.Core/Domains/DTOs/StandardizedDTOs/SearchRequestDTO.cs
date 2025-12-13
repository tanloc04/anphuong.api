using anphuong.Core.Constants;

namespace anphuong.Core.Domains.DTOs.StandardizedDTOs
{
    public class SearchRequestDTO
    {
        public SearchCondition? SearchCondition { get; set; } = null!;

        public PageInfoRequestDTO PageInfo { get; set; } = new()!;
    }
    public class SearchCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class PageInfoRequestDTO
    {
        public int PageNum { get; set; } = Consts.PAGE_NUM_DEFAULT;
        public int PageSize { get; set; } = Consts.PAGE_SIZE_DEFAULT;
    }
}

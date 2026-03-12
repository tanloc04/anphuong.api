using anphuong.Core.Constants;

namespace anphuong.Core.Domains.DTOs.StandardizedDTOs
{
    public class SearchRequestDTO<TCondition>
    {
        public TCondition SearchCondition { get; set; } = default!;
        public PageInfoRequestDTO PageInfo { get; set; } = new();
    }
    public class PageInfoRequestDTO
    {
        public int PageNum { get; set; } = Consts.PAGE_NUM_DEFAULT;
        public int PageSize { get; set; } = Consts.PAGE_SIZE_DEFAULT;
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
    }
}

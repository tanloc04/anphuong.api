using anphuong.Core.Constants;

namespace anphuong.Core.Domains.DTOs.StandardizedDTOs
{
    public class PageInfoRequestDTO
    {
        public int PageNum { get; set; } = Consts.PAGE_NUM_DEFAULT;
        public int PageSize { get; set; } = Consts.PAGE_SIZE_DEFAULT;
    }
}

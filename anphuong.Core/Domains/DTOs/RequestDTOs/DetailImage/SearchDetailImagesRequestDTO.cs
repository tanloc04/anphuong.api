using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage
{
    public class SearchDetailImagesCondition
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SearchDetailImagesRequestDTO
        : SearchRequestDTO<SearchDetailImagesCondition>
    {
    }
}

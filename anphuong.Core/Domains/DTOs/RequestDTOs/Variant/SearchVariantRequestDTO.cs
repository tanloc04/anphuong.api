using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Variant
{
    public class SearchVariantCondition
    {
        public int? ProductId { get; set; }
        public string? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class SearchVariantRequestDTO : SearchRequestDTO<SearchVariantCondition>
    {
    }
}

using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class OrderDetailDTO : DTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public bool IsCustomize { get; set; }
        public int? CustomizeHeight { get; set; }
        public int? CustomizeWidth { get; set; }
        public int? CustomizeLong { get; set; }
        public string? CustomizeMaterial { get; set; }
        public int Quantity { get; set; }
        public double SubTotalPrice { get; set; }
    }
}

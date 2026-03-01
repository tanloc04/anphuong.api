using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Domains.DTOs
{
    public class OrderDetailDTO : DTO
    {
        public int OrderId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantImage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Subtotal { get; set; }
        public bool isCustomized { get; set; }
        public decimal? CustomLongSize { get; set; }
        public decimal? CustomWidthSize { get; set; }
        public decimal? CustomHeightSize { get; set; }
    }
}

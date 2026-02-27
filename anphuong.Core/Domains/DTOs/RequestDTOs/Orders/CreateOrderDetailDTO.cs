namespace anphuong.Core.Domains.DTOs.RequestDTOs.Orders
{
    public class CreateOrderDetailDTO
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public bool IsCustomize { get; set; }
        public int? CustomizeHeight { get; set; }
        public int? CustomizeWidth { get; set; }
        public int? CustomizeLong { get; set; }
        public string? CustomizeMaterial { get; set; }
    }
}

using anphuong.Core.Domains.DTOs.ResponseDTOs.Color;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Domains.DTOs
{
    public class VariantDTO : DTO
    {
        public int Id { get; set; }
        public string? VariantImage { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string SKU { get; set; }

        // Navigation Properties (Tùy cấu trúc DTO của bạn mà gọi cho đúng)
        public ColorDTO? Color { get; set; }
        public MaterialDto? Material { get; set; } // Phải có cái này để show tên Chất liệu
        public InventoryDTO? Inventory { get; set; } // Phải có cái này để show Kho
    }
}

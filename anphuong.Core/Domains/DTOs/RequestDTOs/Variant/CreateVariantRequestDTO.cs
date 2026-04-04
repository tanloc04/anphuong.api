using System.ComponentModel.DataAnnotations;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Variant
{
    public class CreateVariantRequestDTO
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã SKU cho biến thể này!")]
        [MaxLength(50, ErrorMessage = "Mã SKU không được vượt quá 50 ký tự.")]
        public string SKU { get; set; }
        public string VariantImage { get; set; }
    }
}

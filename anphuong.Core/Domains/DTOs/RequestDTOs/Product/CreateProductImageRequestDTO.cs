using anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Product
{
    public class CreateProductImageRequestDTO
    {
        public CreateProductRequestDTO ProductInfo { get; set; } = null!;
        public CreateDetailImageRequestDTO Images { get; set; } = null!;
    }
}

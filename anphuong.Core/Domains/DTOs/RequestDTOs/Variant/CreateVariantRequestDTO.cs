namespace anphuong.Core.Domains.DTOs.RequestDTOs.Variant
{
    public class CreateVariantRequestDTO
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        public string VariantImage { get; set; }
    }
}

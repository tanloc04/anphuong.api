namespace anphuong.Core.Domains.DTOs.API
{
    public class ValidationErrorDTO
    {
        public string? Field { get; set; }
        public List<string>? Message { get; set; }
    }
}

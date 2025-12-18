using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Products
{
    public class UploadImageRequestDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }
}

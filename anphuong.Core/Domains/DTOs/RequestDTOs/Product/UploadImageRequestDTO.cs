using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Product
{
    public class UploadImageRequestDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }
}

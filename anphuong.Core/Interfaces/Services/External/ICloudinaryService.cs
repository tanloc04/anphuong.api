using Microsoft.AspNetCore.Http;

namespace anphuong.Core.Interfaces.Services.External
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string publicId);
    }
}

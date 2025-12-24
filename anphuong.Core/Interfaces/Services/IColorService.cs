using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Color;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Services
{
    public interface IColorService
    {
        public Task<Color> Create(CreateColorRequestDTO request);
        public Task Delete(int id);
        public Task<(IEnumerable<ColorDTO>, int totalItems)> GetAll(SearchColorsRequestDTO request);
        public Task<ColorDTO> Get(int id);
        public Task<ColorDTO> Update(int id, UpdateColorRequestDTO request);
    }
}

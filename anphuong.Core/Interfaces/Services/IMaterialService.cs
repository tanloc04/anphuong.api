using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services
{
    public interface IMaterialService
    {
        Task<(IEnumerable<MaterialDto> pageData, int totalItems)> SearchAsync(SearchMaterialRequestDTO request);
        Task<IEnumerable<MaterialDto>> GetAllAsync();
        Task<MaterialDto?> GetByIdAsync(int id);
        Task<MaterialDto> CreateAsync(CreateMaterialRequestDto request);
        Task<bool> UpdateAsync(int id, CreateMaterialRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}

using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Materials;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Ultilities;
using Mapster;
using System.Linq.Expressions;

namespace anphuong.Service
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _repository;

        public MaterialService(IMaterialRepository materialRepository)
        {
            _repository = materialRepository;
        }
        public async Task<MaterialDto> CreateAsync(CreateMaterialRequestDto request)
        {
            var material = request.Adapt<Material>();
            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = DateTime.UtcNow;
            material.IsDeleted = false;

            await _repository.AddAsync(material);
            return material.Adapt<MaterialDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var material = await _repository.GetAsync(m => m.Id == id && !m.IsDeleted);
            if (material == null) return false;

            material.IsDeleted = true;
            material.UpdatedAt = DateTime.UtcNow;

            return _repository.Update(material);
        }

        public async Task<(IEnumerable<MaterialDto> pageData, int totalItems)> SearchAsync(SearchMaterialRequestDTO request)
        {
            var searchCondition = request?.SearchCondition; // Lấy điều kiện tìm kiếm
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            Expression<Func<Material, bool>> filter = m => true;

            // Lọc theo từ khóa (tìm trong Tên hoặc Mô tả chất liệu)
            if (searchCondition != null && !string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Name.ToLower().Contains(key) ||
                    (x.Description != null && x.Description.ToLower().Contains(key))
                );
            }

            // Lọc theo trạng thái xóa mềm
            bool isDeleted = searchCondition?.IsDeleted ?? false;
            filter = ExpressionUtils.AddFilter(filter, m => m.IsDeleted == isDeleted);

            // Gọi Repository để lấy data có phân trang
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<MaterialDto>>(), totalItems);
        }

        public async Task<IEnumerable<MaterialDto>> GetAllAsync()
        {
            var materials = await _repository.GetAllAsync(m => !m.IsDeleted);
            return materials.Adapt<IEnumerable<MaterialDto>>();
        }

        public async Task<MaterialDto?> GetByIdAsync(int id)
        {
            var material = await _repository.GetAsync(m => m.Id == id && !m.IsDeleted);
            if (material == null) return null;

            return material.Adapt<MaterialDto>();
        }

        public async Task<bool> UpdateAsync(int id, CreateMaterialRequestDto request)
        {
            var material = await _repository.GetAsync(m => m.Id == id && !m.IsDeleted);
            if (material == null) return false;

            material.Name = request.Name;
            material.Description = request.Description;
            material.UpdatedAt = DateTime.UtcNow;

            return _repository.Update(material);
        }
    }
}

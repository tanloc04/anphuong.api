using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Category;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        public Task<Category> Create(CreateCategoryRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<CategoryDTO>, int totalItems)> GetAll(SearchCategoriesRequestDTO request);

        public Task<CategoryDTO> Get(int id);

        public Task<CategoryDTO> Update(int id, UpdateCategoryRequestDTO request);
    }
}

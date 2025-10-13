using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Category;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        public Task<Category> Create(CreateCategoryRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<CategoryDTO>, int totalItems)> GetAll(SearchCategoryRequestDTO request);

        public Task<CategoryDTO> Get(int id);

        public Task<CategoryDTO> Update(int id, UpdateCategoryRequestDTO request);
    }
}

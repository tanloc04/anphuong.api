using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Services
{
    public interface IProductService
    {
        public Task<Product> Create(CreateProductRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchRequestDTO<SearchProductCondition> request);

        public Task<ProductDTO> Get(int id);

        public Task<ProductDTO> Update(int id, UpdateProductRequestDTO request);
    }
}

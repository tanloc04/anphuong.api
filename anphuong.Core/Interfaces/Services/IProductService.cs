using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;

namespace anphuong.Core.Interfaces.Services
{
    public interface IProductService
    {
        public Task<ProductDTO> Create(CreateProductRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchProductsRequestDTO request);
        public Task<ProductDTO> Get(int id);
        public Task<ProductDTO> Update(int id, UpdateProductRequestDTO request);
        public Task<int> GetLowStockCountAsync(int threshold = 5);
    }
}

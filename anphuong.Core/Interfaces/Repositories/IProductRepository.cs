using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product> CreateFullProductAsync(CreateProductRequestDTO request); // Thay đổi chữ ProductDTO thành Product
    }
}

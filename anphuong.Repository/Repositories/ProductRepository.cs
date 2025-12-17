using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace anphuong.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly anphuongDbContext _context;

        public ProductRepository(anphuongDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductListResponseDTO>> GetProductListAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new ProductListResponseDTO
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Price = p.Price,
                    Discount = p.Discount,
                    Description = p.Description,
                    LongSize = p.LongSize,
                    WidthSize = p.WidthSize,
                    HeightSize = p.HeightSize,
                    Material = p.Material,
                    CategoryId = p.CategoryId,
                    VariationId = p.VariationId,
                    Thumbnail = p.DetailImage != null
                        ? p.DetailImage.Thumbnail
                        : null
                })
                .ToListAsync();
        }
    }
}

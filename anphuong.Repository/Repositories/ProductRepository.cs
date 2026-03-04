using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.Data.SqlClient;
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

        public async Task<Product> CreateFullProductAsync(CreateProductRequestDTO request)
        {
            try
            {
                // 1. Kiểm tra CategoryId hợp lệ
                if (!request.CategoryId.HasValue)
                {
                    throw new BusinessException(ErrorDetails.INVALID_CATEGORY_ID);
                }

                // 2. Chỉ tạo Product gốc với đầy đủ các trường mới
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Discount = request.Discount,
                    LongSize = request.LongSize,
                    WidthSize = request.WidthSize,
                    HeightSize = request.HeightSize,
                    isCustomize = request.isCustomize,
                    CategoryId = request.CategoryId.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                // 3. Tạo hình ảnh chi tiết (nếu Admin có tải ảnh lên)
                if (!string.IsNullOrEmpty(request.Image1) || !string.IsNullOrEmpty(request.Image2) ||
                    !string.IsNullOrEmpty(request.Image3) || !string.IsNullOrEmpty(request.Image4))
                {
                    product.DetailImage = new DetailImage
                    {
                        Image1 = request.Image1,
                        Image2 = request.Image2,
                        Image3 = request.Image3,
                        Image4 = request.Image4,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        Product = product
                    };
                }

                // 4. Lưu tất cả xuống Database (EF Core sẽ tự map Khóa ngoại)
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                // 5. Load thêm Category Name để lát nữa Service map DTO cho đẹp
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                // 6. Trả về Entity Product (KHÔNG trả về DTO ở đây nữa)
                return product;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                throw new BusinessException(ErrorDetails.INVALID_CATEGORY_ID);
            }
        }
    }
}
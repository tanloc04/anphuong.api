using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
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
        public async Task<ProductDTO> CreateFullProductAsync(CreateProductRequestDTO request)
        {
            try
            {
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
                    CategoryId = request.CategoryId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };

                var detailImage = new DetailImage
                {
                    Image1 = request.Image1,
                    Image2 = request.Image2,
                    Image3 = request.Image3,
                    Image4 = request.Image4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false,
                    Product = product
                };

                var variant = new Variant
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false,
                    Product = product
                };

                var inventory = new Inventory
                {
                    QuantityInStock = request.Stock,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false,
                    Variant = variant
                };

                _context.DetailImages.Add(detailImage);

                await _context.SaveChangesAsync();

                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                return new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Discount = product.Discount,
                    CategoryId = product.CategoryId,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsDeleted = product.IsDeleted,
                    DetailImageId = detailImage.Id,
                    DetailImage = new ProductDetailImageDTO
                    {
                        Image1 = detailImage.Image1,
                        Image2 = detailImage.Image2,
                        Image3 = detailImage.Image3,
                        Image4 = detailImage.Image4,
                    },
                    Category = new ProductCategoryDTO
                    {
                        Id = product.Category?.Id ?? 0,
                        Name = product.Category?.Name ?? string.Empty
                    },
                    Stock = inventory.QuantityInStock
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                throw new BusinessException(ErrorDetails.INVALID_CATEGORY_ID);
            }
        }
    }
}

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
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var detailImage = new DetailImage
                {
                    Thumbnail = request.Thumbnail,
                    Image1 = request.Image1,
                    Image2 = request.Image2,
                    Image3 = request.Image3,
                    Image4 = request.Image4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };
                _context.DetailImages.Add(detailImage);

                await _context.SaveChangesAsync();

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Discount = request.Discount,
                    Material = request.Material,
                    LongSize = request.LongSize,
                    WidthSize = request.WidthSize,
                    HeightSize = request.HeightSize,
                    CategoryId = request.CategoryId,
                    VariationId = request.VariationId,
                    DetailImageId = detailImage.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };
                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                var inventory = new Inventory
                {
                    QuantityInStock = request.Stock,
                    ProductId = product.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };
                _context.Inventories.Add(inventory);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _context.Entry(product).Reference(p => p.DetailImage).LoadAsync();
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Discount = product.Discount,
                    Material = product.Material,
                    LongSize = product.LongSize,
                    WidthSize = product.WidthSize,
                    HeightSize = product.HeightSize,
                    CategoryId = product.CategoryId,
                    VariationId = product.VariationId,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsDeleted = product.IsDeleted,
                    DetailImageId = detailImage.Id,
                    DetailImage = new ProductDetailImageDTO
                    {
                        Thumbnail = detailImage.Thumbnail,
                        Image1 = detailImage.Image1,
                        Image2 = detailImage.Image2,
                        Image3 = detailImage.Image3,
                        Image4 = detailImage.Image4
                    },
                    Category = new ProductCategoryDTO
                    {
                        Id = product.Category?.Id ?? 0,
                        Name = product.Category?.Name ?? string.Empty
                    },
                    Stock = inventory.QuantityInStock
                };

                return productDTO;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                await transaction.RollbackAsync();
                throw new BusinessException(ErrorDetails.INVALID_CATEGORY_ID);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Ultilities;
using Mapster;

namespace anphuong.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDTO> Create(CreateProductRequestDTO request)
        {
            var product = await _repository.CreateFullProductAsync(request);

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
                DetailImage = new ProductDetailImageDTO
                {
                    Thumbnail = product.DetailImage.Thumbnail,
                    Image1 = product.DetailImage.Image1,
                    Image2 = product.DetailImage.Image2,
                    Image3 = product.DetailImage.Image3,
                    Image4 = product.DetailImage.Image4
                },
                Category = new ProductCategoryDTO
                {
                    Id = product.Category?.Id ?? 0,
                    Name = product.Category?.Name ?? string.Empty
                }
            };

            return productDTO;
        }

        public async Task Delete(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;

            if (!_repository.Update(item))
            {
                throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }

        public async Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchProductsRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchProductCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<Product, bool>> filter = u => true;

            // Only apply keyword filter if keyword exists
            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Name.ToLower().Contains(key) ||
                    (x.Description != null && x.Description.ToLower().Contains(key))
                );
            }

            // Only apply deletion filter if specified (default: return non-deleted)
            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // Query paginated 
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "DetailImage,Category,Inventory");
            var totalItems = await _repository.CountAsync(filter);

            var productDTOs = new List<ProductDTO>();
            foreach (var product in items)
            {
                var dto = product.Adapt<ProductDTO>();
                dto.Stock = product.Inventory?.QuantityInStock ?? 0;
                productDTOs.Add(dto);
            }

            return (productDTOs, totalItems);
        }

        public async Task<ProductDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id, "DetailImage,Category,Inventory")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            var productDTO = item.Adapt<ProductDTO>();
            productDTO.Stock = item.Inventory.QuantityInStock;
            return productDTO;
        }

        public async Task<ProductDTO> Update(int id, UpdateProductRequestDTO request)
        {
            var item = await _repository.GetAsync(id, "DetailImage")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;

            // Reference-type (string) helper
            bool SetIfChanged<T>(T? newValue, Func<T?> getter, Action<T?> setter)
            {
                var oldValue = getter();
                if (newValue != null && !Equals(oldValue, newValue))
                {
                    setter(newValue);
                    return true;
                }
                return false;
            }

            // Non-nullable value-type helper (for product.Price, etc.)
            bool SetIfChangedValue<T>(T? newValue, Func<T> getter, Action<T> setter) where T : struct
            {
                if (newValue.HasValue && !EqualityComparer<T>.Default.Equals(getter(), newValue.Value))
                {
                    setter(newValue.Value);
                    return true;
                }
                return false;
            }

            // Nullable-target value-type helper (for product.DetailImageId, product.CategoryId, product.VariationId)
            bool SetIfChangedNullableValue<T>(T? newValue, Func<T?> getter, Action<T?> setter) where T : struct
            {
                var oldValue = getter();
                if (newValue.HasValue)
                {
                    // update if old is null or different
                    if (!oldValue.HasValue || !EqualityComparer<T>.Default.Equals(oldValue.Value, newValue.Value))
                    {
                        setter(newValue); // set nullable
                        return true;
                    }
                }
                return false;
            }

            // Apply updates
            isChanged |= SetIfChanged(request.Name, () => item.Name, i => item.Name = i);
            isChanged |= SetIfChanged(request.Description, () => item.Description, i => item.Description = i);
            isChanged |= SetIfChanged(request.Material, () => item.Material, i => item.Material = i);
            isChanged |= SetIfChanged(request.DetailImage.Thumbnail, () => item.DetailImage.Thumbnail, i => item.DetailImage.Thumbnail = i);
            isChanged |= SetIfChanged(request.DetailImage.Image1, () => item.DetailImage.Image1, i => item.DetailImage.Image1 = i);
            isChanged |= SetIfChanged(request.DetailImage.Image2, () => item.DetailImage.Image2, i => item.DetailImage.Image2 = i);
            isChanged |= SetIfChanged(request.DetailImage.Image3, () => item.DetailImage.Image3, i => item.DetailImage.Image3 = i);
            isChanged |= SetIfChanged(request.DetailImage.Image4, () => item.DetailImage.Image4, i => item.DetailImage.Image4 = i);

            isChanged |= SetIfChangedValue(request.Price, () => item.Price, i => item.Price = i);
            isChanged |= SetIfChangedValue(request.Discount, () => item.Discount, i => item.Discount = i);
            isChanged |= SetIfChangedValue(request.LongSize, () => item.LongSize, i => item.LongSize = i);
            isChanged |= SetIfChangedValue(request.WidthSize, () => item.WidthSize, i => item.WidthSize = i);
            isChanged |= SetIfChangedValue(request.HeightSize, () => item.HeightSize, i => item.HeightSize = i);
            isChanged |= SetIfChangedValue(request.CategoryId, () => item.CategoryId, i => item.CategoryId = i);

            isChanged |= SetIfChangedNullableValue(request.VariationId, () => item.VariationId, i => item.VariationId = i);

            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
            var itemDTO = item.Adapt<ProductDTO>();
            return itemDTO;
        }
    }
}

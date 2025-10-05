using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Ultilities;
using anphuong.Repository.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task Create(CreateProductRequestDTO request)
        {
            var product = request.Adapt<Product>();
            await _repository.AddAsync(product);
        }

        public async Task Delete(int id)
        {
            var product = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.Now;

            if (!_repository.Update(product))
            {
                throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }

        public async Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchProductRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchCondition();
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

            // Query paginated products
            var products = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (products.Adapt<IEnumerable<ProductDTO>>(), totalItems);
        }

        public async Task<ProductDTO> Get(int id)
        {
            var product = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return product.Adapt<ProductDTO>();
        }

        public async Task Update(int id, UpdateProductRequestDTO request)
        {
            var product = await _repository.GetAsync(id)
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
            isChanged |= SetIfChanged(request.Name, () => product.Name, v => product.Name = v);
            isChanged |= SetIfChanged(request.Description, () => product.Description, v => product.Description = v);
            isChanged |= SetIfChanged(request.Material, () => product.Material, v => product.Material = v);

            isChanged |= SetIfChangedValue(request.Price, () => product.Price, v => product.Price = v);
            isChanged |= SetIfChangedValue(request.Discount, () => product.Discount, v => product.Discount = v);
            isChanged |= SetIfChangedValue(request.LongSize, () => product.LongSize, v => product.LongSize = v);
            isChanged |= SetIfChangedValue(request.WidthSize, () => product.WidthSize, v => product.WidthSize = v);
            isChanged |= SetIfChangedValue(request.HeightSize, () => product.HeightSize, v => product.HeightSize = v);

            // These 3 target properties are nullable (int?) on Product — use nullable helper
            isChanged |= SetIfChangedNullableValue(request.DetailImageId, () => product.DetailImageId, v => product.DetailImageId = v);
            isChanged |= SetIfChangedNullableValue(request.CategoryId, () => product.CategoryId, v => product.CategoryId = v);
            isChanged |= SetIfChangedNullableValue(request.VariationId, () => product.VariationId, v => product.VariationId = v);

            if (isChanged)
            {
                product.UpdatedAt = DateTime.UtcNow;
                if (! _repository.Update(product))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }

    }
}

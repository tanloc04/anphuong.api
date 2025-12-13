using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Core.Ultilities;
using Mapster;

namespace anphuong.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IDetailImageRepository _detailImageRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IProductRepository repository,
            IDetailImageRepository detailImageRepository,
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _detailImageRepository = detailImageRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Product> Create(CreateProductRequestDTO request)
        {
            var detailImageDTO = new DetailImageDTO
            {
                Thumbnail = request.Thumbnail,
                Image1 = request.Image1,
                Image2 = request.Image2,
                Image3 = request.Image3,
                Image4 = request.Image4
            };

            var detailImage = new DetailImage
            {
                Thumbnail = detailImageDTO.Thumbnail,
                Image1 = detailImageDTO.Image1,
                Image2 = detailImageDTO.Image2,
                Image3 = detailImageDTO.Image3,
                Image4 = detailImageDTO.Image4
            };

            await _detailImageRepository.AddAsync(detailImage);

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
                DetailImageId = detailImage.Id
            };

            await _repository.AddAsync(product);
            return product;
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

            // Query paginated 
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<ProductDTO>>(), totalItems);
        }

        public async Task<ProductDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<ProductDTO>();
        }

        public async Task<ProductDTO> Update(int id, UpdateProductRequestDTO request)
        {
            var item = await _repository.GetAsync(id)
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
            isChanged |= SetIfChanged(request.Thumnail, () => item.DetailImage.Thumbnail, i => item.DetailImage.Thumbnail = i);
            isChanged |= SetIfChanged(request.Image1, () => item.DetailImage.Image1, i => item.DetailImage.Image1 = i);
            isChanged |= SetIfChanged(request.Image2, () => item.DetailImage.Image2, i => item.DetailImage.Image2 = i);
            isChanged |= SetIfChanged(request.Image3, () => item.DetailImage.Image3, i => item.DetailImage.Image3 = i);
            isChanged |= SetIfChanged(request.Image4, () => item.DetailImage.Image4, i => item.DetailImage.Image4 = i);

            isChanged |= SetIfChangedValue(request.Price, () => item.Price, i => item.Price = i);
            isChanged |= SetIfChangedValue(request.Discount, () => item.Discount, i => item.Discount = i);
            isChanged |= SetIfChangedValue(request.LongSize, () => item.LongSize, i => item.LongSize = i);
            isChanged |= SetIfChangedValue(request.WidthSize, () => item.WidthSize, i => item.WidthSize = i);
            isChanged |= SetIfChangedValue(request.HeightSize, () => item.HeightSize, i => item.HeightSize = i);

            isChanged |= SetIfChangedNullableValue(request.CategoryId, () => item.CategoryId, i => item.CategoryId = i);
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

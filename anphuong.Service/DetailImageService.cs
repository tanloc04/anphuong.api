using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage;
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
    public class DetailImageService : IDetailImageService
    {
        private readonly IDetailImageRepository _repository;

        public DetailImageService(IDetailImageRepository repository)
        {
            _repository = repository;
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

        public async Task<(IEnumerable<DetailImageDTO>, int totalItems)> GetAll(SearchDetailImageRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchDetailImageCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<DetailImage, bool>> filter = u => true;

            // Only apply deletion filter if specified (default: return non-deleted)
            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // Query paginated 
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<DetailImageDTO>>(), totalItems);
        }

        public async Task<DetailImageDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<DetailImageDTO>();
        }

        public async Task<DetailImageDTO> Update(int id, UpdateDetailImageRequestDTO request)
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
            isChanged |= SetIfChanged(request.Thumnail, () => item.Thumbnail, i => item.Thumbnail = i);
            isChanged |= SetIfChanged(request.Image1, () => item.Image1, i => item.Image1 = i);
            isChanged |= SetIfChanged(request.Image2, () => item.Image2, i => item.Image2 = i);
            isChanged |= SetIfChanged(request.Image3, () => item.Image3, i => item.Image3 = i);
            isChanged |= SetIfChanged(request.Image4, () => item.Image4, i => item.Image4 = i);
            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
            var itemDTO = item.Adapt<DetailImageDTO>();
            return itemDTO;
        }
    }
}

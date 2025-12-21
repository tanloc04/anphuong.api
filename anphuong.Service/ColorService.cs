using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Color;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Ultilities;
using Mapster;
using System.Linq.Expressions;


namespace anphuong.Service
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _repository;
        public ColorService(IColorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Color> Create(CreateColorRequestDTO request)
        {
            var item = request.Adapt<Color>();
            await _repository.AddAsync(item);
            return item;
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

        public async Task<(IEnumerable<ColorDTO>, int totalItems)> GetAll(SearchColorsRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchColorsCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<Color, bool>> filter = u => true;

            // Only apply keyword filter if keyword exists
            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Name.ToLower().Contains(key) ||
                    (x.HexCode != null && x.HexCode.ToLower().Contains(key))
                );
            }

            // Only apply deletion filter if specified (default: return non-deleted)
            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // Query paginated products
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<ColorDTO>>(), totalItems);
        }

        public async Task<ColorDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<ColorDTO>();
        }

        public async Task<ColorDTO> Update(int id, UpdateColorRequestDTO request)
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
            isChanged |= SetIfChanged(request.HexCode, () => item.HexCode, i => item.HexCode = i);
            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
            var itemDTO = item.Adapt<ColorDTO>();
            return itemDTO;
        }
    }
}

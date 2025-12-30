using System.Linq.Expressions;
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

            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChanged(request.Name, () => item.Name, i => item.Name = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.HexCode, () => item.HexCode, i => item.HexCode = i);
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

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

        public async Task<(IEnumerable<DetailImageDTO>, int totalItems)> GetAll(SearchDetailImagesRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchDetailImagesCondition();
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

            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChanged(request.Thumbnail, () => item.Thumbnail, i => item.Thumbnail = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Image1, () => item.Image1, i => item.Image1 = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Image2, () => item.Image2, i => item.Image2 = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Image3, () => item.Image3, i => item.Image3 = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Image4, () => item.Image4, i => item.Image4 = i);
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

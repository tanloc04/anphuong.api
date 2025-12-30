using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Variant;
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
    public class VariantService : IVariantService
    {
        private readonly IVariantRepository _repository;

        public VariantService(IVariantRepository repository)
        {
            _repository = repository;
        }
        public async Task<VariantDTO> Create(CreateVariantRequestDTO request)
        {
            var entity = request.Adapt<Variant>();
            await _repository.AddAsync(entity);

            return entity.Adapt<VariantDTO>();
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

        public async Task<(IEnumerable<VariantDTO>, int totalItems)> GetAll(SearchVariantRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchVariantCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<Variant, bool>> filter = u => true;

            // Only apply keyword filter if keyword exists
            if (searchCondition.ProductId.HasValue)
            {
                var searchId = searchCondition.ProductId.Value;
                filter = ExpressionUtils.AddFilter(filter, x => x.ProductId == searchId);
            }
            // Only apply deletion filter if specified (default: return non-deleted)
            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // Query paginated 
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "Color,Product");
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<VariantDTO>>(), totalItems);
        }

        public async Task<VariantDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<VariantDTO>();
        }

        public async Task<VariantDTO> Update(int id, UpdateVariantRequestDTO request)
        {
            var item = await _repository.GetAsync(id)
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;

            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ProductId, () => item.ProductId, i => item.ProductId = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ColorId, () => item.ColorId, i => item.ColorId = i);
            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
            var itemDTO = item.Adapt<VariantDTO>();
            return itemDTO;
        }
    }
}

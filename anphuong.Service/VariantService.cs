using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Variant;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Color;
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
    public class VariantService : IVariantService
    {
        private readonly IVariantRepository _repository;

        public VariantService(IVariantRepository repository)
        {
            _repository = repository;
        }

        public async Task<VariantDTO> Create(CreateVariantRequestDTO request)
        {
            var entity = new Variant
            {
                ProductId = request.ProductId,
                ColorId = request.ColorId,
                VariantImage = request.VariantImage,

                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

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
            var searchCondition = request?.SearchCondition ?? new SearchVariantCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            Expression<Func<Variant, bool>> filter = u => true;

            if (searchCondition.ProductId.HasValue && searchCondition.ProductId.Value > 0)
            {
                var searchId = searchCondition.ProductId.Value;
                filter = ExpressionUtils.AddFilter(filter, x => x.ProductId == searchId);
            }

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "Color,Product");
            var totalItems = await _repository.CountAsync(filter);

            var resultDTOs = items.Select(x => new VariantDTO
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ColorId = x.ColorId,
                VariantImage = x.VariantImage,
                Color = x.Color == null ? null : new ColorNameDTO
                {
                    Name = x.Color.Name,
                    HexCode = x.Color.HexCode
                }
            });

            return (resultDTOs, totalItems);
        }

        public async Task<VariantDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<VariantDTO>();
        }

        public async Task<VariantDTO> Update(int id, UpdateVariantRequestDTO request)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;

            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ProductId, () => item.ProductId, i => item.ProductId = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ColorId, () => item.ColorId, i => item.ColorId = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.VariantImage, () => item.VariantImage, i => item.VariantImage = i);

            if (isChanged)
            {
                item.UpdatedAt = DateTime.Now;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }

            return item.Adapt<VariantDTO>();
        }
    }
}
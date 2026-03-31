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
            var variant = new Variant
            {
                ProductId = request.ProductId,
                ColorId = request.ColorId,
                MaterialId = request.MaterialId,
                Price = request.Price,
                SKU = request.SKU,
                VariantImage = request.VariantImage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,

                Inventory = new Inventory
                {
                    QuantityInStock = request.Stock,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            await _repository.AddAsync(variant);

            return variant.Adapt<VariantDTO>();
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

            if (searchCondition.ProductId.HasValue)
            {
                var searchId = searchCondition.ProductId.Value;
                filter = ExpressionUtils.AddFilter(filter, x => x.ProductId == searchId);
            }

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "Color,Material,Inventory");
            var totalItems = await _repository.CountAsync(filter);

            TypeAdapterConfig<Variant, VariantDTO>.NewConfig()
                .Map(dest => dest.QuantityInStock, src => src.Inventory != null ? src.Inventory.QuantityInStock : 0);

            return (items.Adapt<IEnumerable<VariantDTO>>(), totalItems);
        }

        public async Task<VariantDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<VariantDTO>();
        }

        public async Task<VariantDTO> Update(int id, UpdateVariantRequestDTO request)
        {
            var item = await _repository.GetAsync(id, "VariantImage")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;

            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ProductId, () => item.ProductId, i => item.ProductId = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.ColorId, () => item.ColorId, i => item.ColorId = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.VariantImage, () => item.VariantImage, i => item.VariantImage = i);
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

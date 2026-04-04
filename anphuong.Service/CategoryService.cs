using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Category;
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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Category> Create(CreateCategoryRequestDTO request)
        {
            var item = request.Adapt<Category>();
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

        public async Task<(IEnumerable<CategoryDTO>, int totalItems)> GetAll(SearchCategoriesRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchCategoriesCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<Category, bool>> filter = u => true;

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

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = q =>
            {
                if (!string.IsNullOrEmpty(pageInfo.SortBy))
                {
                    bool isDesc = pageInfo.SortDesc ?? false;
                    return pageInfo.SortBy.ToLower() switch
                    {
                        "name" => isDesc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name),
                        "description" => isDesc ? q.OrderByDescending(x => x.Description) : q.OrderBy(x => x.Description),
                        _ => q.OrderByDescending(x => x.CreatedAt) // Mặc định
                    };
                }
                return q.OrderByDescending(x => x.CreatedAt); // Mặc định
            };

            // Query paginated products
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "", orderBy);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<CategoryDTO>>(), totalItems);
        }

        public async Task<CategoryDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            return item.Adapt<CategoryDTO>();
        }

        public async Task<CategoryDTO> Update(int id, UpdateCategoryRequestDTO request)
        {
            var item = await _repository.GetAsync(id)
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;
            isChanged |= GenericHelperUtils.SetIfChanged(request.Name, () => item.Name, i => item.Name = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Description, () => item.Description, i => item.Description = i);

            isChanged |= GenericHelperUtils.SetIfChanged(request.ImageUrl, () => item.ImageUrl, i => item.ImageUrl = i);

            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }

            var itemDTO = item.Adapt<CategoryDTO>();
            return itemDTO;
        }
    }
}

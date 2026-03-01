using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDTO> Create(CreateProductRequestDTO request)
        {
            var product = await _repository.CreateFullProductAsync(request);

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                // FIX: Xóa Material và VariationId vì không còn nằm trong Product
                LongSize = product.LongSize,
                WidthSize = product.WidthSize,
                HeightSize = product.HeightSize,
                
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsDeleted = product.IsDeleted,
                DetailImage = product.DetailImage != null ? new ProductDetailImageDTO
                {
                    Image1 = product.DetailImage.Image1,
                    Image2 = product.DetailImage.Image2,
                    Image3 = product.DetailImage.Image3,
                    Image4 = product.DetailImage.Image4
                } : null,
                Category = new ProductCategoryDTO
                {
                    Id = product.Category?.Id ?? 0,
                    Name = product.Category?.Name ?? string.Empty
                },
                Stock = request.Stock
            };

            return productDTO;
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

        public async Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchProductsRequestDTO request)
        {
            var searchCondition = request?.SearchCondition ?? new SearchProductCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            Expression<Func<Product, bool>> filter = u => true;

            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Name.ToLower().Contains(key) ||
                    (x.Description != null && x.Description.ToLower().Contains(key))
                );
            }

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // FIX: Đổi Include từ Inventory sang Variants.Inventory
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "DetailImage,Category,Variants.Inventory");
            var totalItems = await _repository.CountAsync(filter);

            var productDTOs = new List<ProductDTO>();
            foreach (var product in items)
            {
                var dto = product.Adapt<ProductDTO>();

                // FIX: Tính tổng tồn kho của tất cả các biến thể thuộc sản phẩm này
                dto.Stock = product.Variants?.Sum(v => v.Inventory?.QuantityInStock ?? 0) ?? 0;

                productDTOs.Add(dto);
            }

            return (productDTOs, totalItems);
        }

        public async Task<ProductDTO> Get(int id)
        {
            // FIX: Đổi Include từ Inventory sang Variants.Inventory
            var item = await _repository.GetAsync(id, "DetailImage,Category,Variants.Inventory")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            var productDTO = item.Adapt<ProductDTO>();

            // FIX: Tính tổng tồn kho
            productDTO.Stock = item.Variants?.Sum(v => v.Inventory?.QuantityInStock ?? 0) ?? 0;

            return productDTO;
        }

        public async Task<ProductDTO> Update(int id, UpdateProductRequestDTO request)
        {
            var item = await _repository.GetAsync(id, "DetailImage,Category")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;

            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChanged(request.Name, () => item.Name, i => item.Name = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Description, () => item.Description, i => item.Description = i);

            if (item.DetailImage != null && request.DetailImage != null)
            {
                isChanged |= GenericHelperUtils.SetIfChanged(request.DetailImage.Image1, () => item.DetailImage.Image1, i => item.DetailImage.Image1 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.DetailImage.Image2, () => item.DetailImage.Image2, i => item.DetailImage.Image2 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.DetailImage.Image3, () => item.DetailImage.Image3, i => item.DetailImage.Image3 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.DetailImage.Image4, () => item.DetailImage.Image4, i => item.DetailImage.Image4 = i);
            }

            isChanged |= GenericHelperUtils.SetIfChangedValue(request.Price, () => item.Price, i => item.Price = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.Discount, () => item.Discount, i => item.Discount = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.LongSize, () => item.LongSize, i => item.LongSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.WidthSize, () => item.WidthSize, i => item.WidthSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.HeightSize, () => item.HeightSize, i => item.HeightSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.CategoryId, () => item.CategoryId, i => item.CategoryId = i);

            // FIX: Bỏ các dòng Update Material và VariationId vì chúng không thuộc Product nữa

            if (isChanged)
            {
                item.UpdatedAt = DateTime.Now;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }

            var itemDTO = item.Adapt<ProductDTO>();
            return itemDTO;
        }
    }
}
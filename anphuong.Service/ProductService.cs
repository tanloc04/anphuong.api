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
using anphuong.Repository.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
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

                LongSize = product.LongSize,
                WidthSize = product.WidthSize,
                HeightSize = product.HeightSize,
                isCustomize = product.isCustomize,

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

                // Sản phẩm mới tạo chắc chắn chưa có Biến thể và Kho
                TotalStock = 0,
                IsMissingVariants = true
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
                var key = searchCondition.Keyword.ToLower().Trim();

                if (key.StartsWith("ap-") && int.TryParse(key.Substring(3), out int parsedId))
                {
                    // Nếu đúng chuẩn AP-xxx thì ưu tiên tìm theo ID (Mã SP), hoặc lỡ có trong Tên/Mô tả
                    filter = ExpressionUtils.AddFilter(filter, x =>
                        x.Id == parsedId ||
                        x.Name.ToLower().Contains(key) ||
                        (x.Description != null && x.Description.ToLower().Contains(key))
                    );
                }
                else
                {
                    // Nếu gõ chữ bình thường thì tìm theo Tên và Mô tả như cũ
                    filter = ExpressionUtils.AddFilter(filter, x =>
                        x.Name.ToLower().Contains(key) ||
                        (x.Description != null && x.Description.ToLower().Contains(key))
                    );
                }
            }

            if (searchCondition.CategoryId.HasValue && searchCondition.CategoryId.Value > 0)
            {
                filter = ExpressionUtils.AddFilter(filter, x => x.CategoryId == searchCondition.CategoryId.Value);
            }

            
            if (searchCondition.MinPrice.HasValue)
            {
                filter = ExpressionUtils.AddFilter(filter, x => x.Price >= searchCondition.MinPrice.Value);
            }

            if (searchCondition.MaxPrice.HasValue)
            {
                filter = ExpressionUtils.AddFilter(filter, x => x.Price <= searchCondition.MaxPrice.Value);
            }

            if (searchCondition.StartDate.HasValue)
            {
                var start = searchCondition.StartDate.Value.Date;
                filter = ExpressionUtils.AddFilter(filter, x => x.CreatedAt >= start);
            }

            if (searchCondition.EndDate.HasValue)
            {
                var end = searchCondition.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                filter = ExpressionUtils.AddFilter(filter, x => x.CreatedAt <= end);
            }

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = q =>
            {
                if (!string.IsNullOrEmpty(pageInfo.SortBy))
                {
                    bool isDesc = pageInfo.SortDesc ?? false;
                    return pageInfo.SortBy.ToLower() switch
                    {
                        "name" => isDesc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name),
                        "price" => isDesc ? q.OrderByDescending(x => x.Price) : q.OrderBy(x => x.Price),
                        "totalstock" => isDesc ? q.OrderByDescending(x => x.Variants.Sum(v => v.Inventory.QuantityInStock)) : q.OrderBy(x => x.Variants.Sum(v => v.Inventory.QuantityInStock)), // Sắp xếp theo số lượng kho
                        _ => q.OrderByDescending(x => x.CreatedAt) // Mặc định sort theo ngày tạo mới nhất
                    };
                }
                return q.OrderByDescending(x => x.CreatedAt); // Mặc định
            };

            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "DetailImage,Category,Variants.Inventory", orderBy);

            var totalItems = await _repository.CountAsync(filter);

            var productDTOs = new List<ProductDTO>();
            foreach (var product in items)
            {
                var dto = product.Adapt<ProductDTO>();

                dto.Thumbnail = product.DetailImage?.Image1;

                dto.TotalStock = product.Variants?.Sum(v => v.Inventory?.QuantityInStock ?? 0) ?? 0;

                dto.IsMissingVariants = product.Variants == null || !product.Variants.Any();

                productDTOs.Add(dto);
            }

            return (productDTOs, totalItems);
        }

        public async Task<IEnumerable<object>> GetAutocompleteSuggestionsAsync(string keyword)
        {
            var products = await _repository.GetAutocompleteSuggestionsAsync(keyword);

            keyword = keyword.ToLower().Trim();

            var result = products.Select(p =>
            {
                // Ưu tiên tìm biến thể (Variant) khớp với từ khóa SKU khách gõ, nếu không có thì lấy biến thể đầu tiên
                var matchedVariant = p.Variants.FirstOrDefault(v => v.SKU != null && v.SKU.ToLower().Contains(keyword))
                                     ?? p.Variants.FirstOrDefault();

                return new
                {
                    Id = p.Id, // ID của Product để bấm vào chuyển trang chi tiết
                    ProductCode = matchedVariant?.SKU ?? "N/A", // Trả về SKU thay cho ProductCode
                    ProductName = p.Name,
                    Price = matchedVariant?.Price ?? 0, // Giá lấy từ Variant
                    ImageUrl = matchedVariant?.VariantImage ?? "", // Ảnh lấy từ Variant
                    CategoryName = p.Category?.Name ?? ""
                };
            });

            return result;
        }

        public async Task<ProductDTO> Get(int id)
        {
            var item = await _repository.GetAsync(id, "DetailImage,Category,Variants.Inventory,Variants.Color,Variants.Material,Reviews")
    ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            var productDTO = item.Adapt<ProductDTO>();

            productDTO.Thumbnail = item.DetailImage?.Image1;

            productDTO.TotalStock = item.Variants?.Sum(v => v.Inventory?.QuantityInStock ?? 0) ?? 0;
            productDTO.IsMissingVariants = item.Variants == null || !item.Variants.Any();

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

            // Xử lý Update Hình ảnh chi tiết từ các thuộc tính phẳng
            if (item.DetailImage != null)
            {
                isChanged |= GenericHelperUtils.SetIfChanged(request.Image1, () => item.DetailImage.Image1, i => item.DetailImage.Image1 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.Image2, () => item.DetailImage.Image2, i => item.DetailImage.Image2 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.Image3, () => item.DetailImage.Image3, i => item.DetailImage.Image3 = i);
                isChanged |= GenericHelperUtils.SetIfChanged(request.Image4, () => item.DetailImage.Image4, i => item.DetailImage.Image4 = i);

                if (isChanged) item.DetailImage.UpdatedAt = DateTime.Now;
            }
            else if (!string.IsNullOrEmpty(request.Image1) || !string.IsNullOrEmpty(request.Image2) ||
                     !string.IsNullOrEmpty(request.Image3) || !string.IsNullOrEmpty(request.Image4))
            {
                item.DetailImage = new DetailImage
                {
                    Image1 = request.Image1,
                    Image2 = request.Image2,
                    Image3 = request.Image3,
                    Image4 = request.Image4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                isChanged = true;
            }

            isChanged |= GenericHelperUtils.SetIfChangedValue(request.Price, () => item.Price, i => item.Price = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.Discount, () => item.Discount, i => item.Discount = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.LongSize, () => item.LongSize, i => item.LongSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.WidthSize, () => item.WidthSize, i => item.WidthSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.HeightSize, () => item.HeightSize, i => item.HeightSize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.isCustomize, () => item.isCustomize, i => item.isCustomize = i);
            isChanged |= GenericHelperUtils.SetIfChangedValue(request.CategoryId, () => item.CategoryId, i => item.CategoryId = i);

            if (isChanged)
            {
                item.UpdatedAt = DateTime.Now;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }

            var itemDTO = item.Adapt<ProductDTO>();

            // Do hàm Update không Include Variants để tối ưu, trả về TotalStock = 0 tạm thời
            // (Thường gọi Update xong UI sẽ refetch lại hàm GetById nên không sao)
            itemDTO.TotalStock = 0;

            return itemDTO;
        }

        public async Task<int> GetLowStockCountAsync(int threshold = 5)
        {
            return await _repository.CountLowStockProductsAsync(threshold);
        }

    }
}
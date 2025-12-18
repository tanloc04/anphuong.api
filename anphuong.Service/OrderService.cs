using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Orders;
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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public OrderService(IOrderRepository repository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<OrderDTO> PlaceOrderAsync(CreateOrderRequestDTO request)
        {
            var groupedItems = request.OrderDetails
             .GroupBy(x => x.ProductId)
             .Select(g => new OrderItemQuantity
             {
                 ProductId = g.Key,
                 Quantity = g.Sum(x => x.Quantity)
             })
             .ToList();

            await _inventoryRepository.CheckStockAsync(groupedItems);


            var order = new Order
            {
                CustomerId = request.CustomerId,
                PaymentMethod = request.PaymentMethod,
                Status = 1,
                ShippingDate = request.ShippingDate,
                TotalPrice = 0
            };

            await _repository.AddAsync(order);


            var products = new Dictionary<int, Product>();

            foreach (var item in groupedItems)
            {
                var product = await _productRepository.GetAsync(item.ProductId, "DetailImage")
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

                products.Add(item.ProductId, product);
            }


            var orderDetails = new List<OrderDetail>();
            double totalPrice = 0;

            foreach (var d in request.OrderDetails)
            {
                var product = products[d.ProductId];
                var subTotal = product.Price * d.Quantity;

                orderDetails.Add(new OrderDetail
                {
                    Order = order,
                    Product = product,
                    ProductId = product.Id,

                    Quantity = d.Quantity,
                    SubTotalPrice = subTotal,

                    IsCustomize = d.IsCustomize,
                    CustomizeHeight = d.IsCustomize ? d.CustomizeHeight : null,
                    CustomizeWidth = d.IsCustomize ? d.CustomizeWidth : null,
                    CustomizeLong = d.IsCustomize ? d.CustomizeLong : null,
                    CustomizeMaterial = d.IsCustomize ? d.CustomizeMaterial : null
                });

                totalPrice += subTotal;
            }

            await _repository.AddRangeOrderDetailsAsync(orderDetails);

            await _inventoryRepository.ReduceStockBatchAsync(groupedItems);


            order.TotalPrice = totalPrice;
            _repository.Update(order);

            return new OrderDTO
            {
                Id = order.Id,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                ShippingDate = order.ShippingDate,
                TotalPrice = order.TotalPrice,
                CustomerId = order.CustomerId,

                OrderDetails = orderDetails.Select(od => new OrderDetailDTO
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Thumbnail = od.Product?.DetailImage?.Thumbnail,

                    Quantity = od.Quantity,
                    SubTotalPrice = od.SubTotalPrice,

                    IsCustomize = od.IsCustomize,
                    CustomizeHeight = od.CustomizeHeight,
                    CustomizeWidth = od.CustomizeWidth,
                    CustomizeLong = od.CustomizeLong,
                    CustomizeMaterial = od.CustomizeMaterial
                }).ToList()
            };
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

        public async Task<(IEnumerable<OrderDTO> Orders, double TotalPrice, int TotalItems)> GetAll(
            SearchOrderRequestDTO request)
        {
            var searchCondition = request?.SearchCondition ?? new SearchOrderCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            Expression<Func<Order, bool>> filter = u => true;

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchCondition.Status) &&
                int.TryParse(searchCondition.Status, out int statusValue))
            {
                filter = ExpressionUtils.AddFilter(filter, u => u.Status == statusValue);
            }

            if (!string.IsNullOrWhiteSpace(searchCondition.Keyword))
            {
                filter = ExpressionUtils.AddFilter(filter, u =>
                    u.Customer.FullName.Contains(searchCondition.Keyword));
            }

            if (searchCondition.FromDate.HasValue)
            {
                filter = ExpressionUtils.AddFilter(filter, u => u.CreatedAt >= searchCondition.FromDate.Value);
            }

            if (searchCondition.ToDate.HasValue)
            {
                filter = ExpressionUtils.AddFilter(filter, u => u.CreatedAt <= searchCondition.ToDate.Value);
            }

            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "OrderDetails.Product.DetailImage,Customer.User");

            var totalItems = await _repository.CountAsync(filter);

            double totalPrice = 0;

            if (searchCondition.isTotalPrice.HasValue && searchCondition.isTotalPrice.Value)
            {
                totalPrice = items.Sum(o => o.TotalPrice);
            }

            TypeAdapterConfig<OrderDetail, OrderDetailDTO>.NewConfig()
                    .Map(dest => dest.Thumbnail, src => src.Product.DetailImage!.Thumbnail);
            TypeAdapterConfig<Customer, CustomerDTO>.NewConfig()
                  .Map(dest => dest.Fullname, src => src.FullName)
                  .Map(dest => dest.Phone, src => src.Phone)
                  .Map(dest => dest.CustomerAddress, src => src.CustomerAddress)
                  .Map(dest => dest.Email, src => src.User.Email);

            var ordersDto = items.Adapt<IEnumerable<OrderDTO>>();

            return (ordersDto, totalPrice, totalItems);
        }

        public async Task<OrderDTO> Get(int id)
        {
            var item = await _repository.GetAsync(
             id, "OrderDetails.Product.DetailImage,Customer.User"
            ) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);
            TypeAdapterConfig<OrderDetail, OrderDetailDTO>.NewConfig()
                .Map(dest => dest.Thumbnail, src => src.Product.DetailImage!.Thumbnail);
            TypeAdapterConfig<Customer, CustomerDTO>.NewConfig()
              .Map(dest => dest.Fullname, src => src.FullName)
              .Map(dest => dest.Phone, src => src.Phone)
              .Map(dest => dest.CustomerAddress, src => src.CustomerAddress)
              .Map(dest => dest.Email, src => src.User.Email);
            return item.Adapt<OrderDTO>();
        }

        //public async Task<OrderDTO> Update(int id, UpdateProductRequestDTO request)
        //{
        //    var item = await _repository.GetAsync(id, "OrderDetail")
        //        ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

        //    bool isChanged = false;

        //    // Reference-type (string) helper
        //    bool SetIfChanged<T>(T? newValue, Func<T?> getter, Action<T?> setter)
        //    {
        //        var oldValue = getter();
        //        if (newValue != null && !Equals(oldValue, newValue))
        //        {
        //            setter(newValue);
        //            return true;
        //        }
        //        return false;
        //    }

        //    // Non-nullable value-type helper (for product.Price, etc.)
        //    bool SetIfChangedValue<T>(T? newValue, Func<T> getter, Action<T> setter) where T : struct
        //    {
        //        if (newValue.HasValue && !EqualityComparer<T>.Default.Equals(getter(), newValue.Value))
        //        {
        //            setter(newValue.Value);
        //            return true;
        //        }
        //        return false;
        //    }

        //    // Nullable-target value-type helper (for product.DetailImageId, product.CategoryId, product.VariationId)
        //    bool SetIfChangedNullableValue<T>(T? newValue, Func<T?> getter, Action<T?> setter) where T : struct
        //    {
        //        var oldValue = getter();
        //        if (newValue.HasValue)
        //        {
        //            // update if old is null or different
        //            if (!oldValue.HasValue || !EqualityComparer<T>.Default.Equals(oldValue.Value, newValue.Value))
        //            {
        //                setter(newValue); // set nullable
        //                return true;
        //            }
        //        }
        //        return false;
        //    }

        //    // Apply updates
        //    isChanged |= SetIfChanged(request.Name, () => item.Name, i => item.Name = i);
        //    isChanged |= SetIfChanged(request.Description, () => item.Description, i => item.Description = i);
        //    isChanged |= SetIfChanged(request.Material, () => item.Material, i => item.Material = i);
        //    isChanged |= SetIfChanged(request.DetailImage.Thumbnail, () => item.DetailImage.Thumbnail, i => item.DetailImage.Thumbnail = i);
        //    isChanged |= SetIfChanged(request.DetailImage.Image1, () => item.DetailImage.Image1, i => item.DetailImage.Image1 = i);
        //    isChanged |= SetIfChanged(request.DetailImage.Image2, () => item.DetailImage.Image2, i => item.DetailImage.Image2 = i);
        //    isChanged |= SetIfChanged(request.DetailImage.Image3, () => item.DetailImage.Image3, i => item.DetailImage.Image3 = i);
        //    isChanged |= SetIfChanged(request.DetailImage.Image4, () => item.DetailImage.Image4, i => item.DetailImage.Image4 = i);

        //    isChanged |= SetIfChangedValue(request.Price, () => item.Price, i => item.Price = i);
        //    isChanged |= SetIfChangedValue(request.Discount, () => item.Discount, i => item.Discount = i);
        //    isChanged |= SetIfChangedValue(request.LongSize, () => item.LongSize, i => item.LongSize = i);
        //    isChanged |= SetIfChangedValue(request.WidthSize, () => item.WidthSize, i => item.WidthSize = i);
        //    isChanged |= SetIfChangedValue(request.HeightSize, () => item.HeightSize, i => item.HeightSize = i);
        //    isChanged |= SetIfChangedValue(request.CategoryId, () => item.CategoryId, i => item.CategoryId = i);

        //    isChanged |= SetIfChangedNullableValue(request.VariationId, () => item.VariationId, i => item.VariationId = i);

        //    if (isChanged)
        //    {
        //        item.UpdatedAt = DateTime.UtcNow;
        //        if (!_repository.Update(item))
        //            throw new BusinessException(ErrorDetails.DEFAULT);
        //    }
        //    var itemDTO = item.Adapt<ProductDTO>();
        //    return itemDTO;
        //}
    }
}

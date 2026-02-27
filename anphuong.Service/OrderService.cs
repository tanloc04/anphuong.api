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

        // FIX 1: Inject thêm IVariantRepository để query Variant
        private readonly IVariantRepository _variantRepository;

        public OrderService(IOrderRepository repository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            IVariantRepository variantRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _variantRepository = variantRepository;
        }

        public async Task<OrderDTO> PlaceOrderAsync(CreateOrderRequestDTO request)
        {
            var groupedItems = request.OrderDetails
                .GroupBy(x => x.VariantId)
                .Select(g => new OrderItemQuantity
                {
                    VariantId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                }).ToList();

            await _inventoryRepository.CheckStockAsync(groupedItems);

            var order = new Order
            {
                CustomerId = request.CustomerId,
                PaymentMethod = request.PaymentMethod,
                Status = 1,
                ShippingDate = request.ShippingDate,
                TotalPrice = 0, // Sẽ cộng dồn ở dưới
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                OrderDetails = new List<OrderDetail>()
            };

            var variants = new Dictionary<int, Variant>();
            foreach (var item in groupedItems)
            {
                var variant = await _variantRepository.GetAsync(item.VariantId, "Product.DetailImage")
                    ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);
                variants.Add(item.VariantId, variant);
            }

            double totalPrice = 0;

            foreach (var d in request.OrderDetails)
            {
                var variant = variants[d.VariantId];
                var unitPrice = variant.Product.Price;
                var subTotal = unitPrice * d.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    VariantId = d.VariantId,
                    Quantity = d.Quantity,
                    UnitPrice = unitPrice,
                    SubTotal = subTotal,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                });

                totalPrice += subTotal;
            }

            order.TotalPrice = totalPrice;

            await _repository.AddAsync(order);

            await _inventoryRepository.ReduceStockBatchAsync(groupedItems);

            return new OrderDTO
            {
                Id = order.Id,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                ShippingDate = order.ShippingDate,
                TotalPrice = order.TotalPrice,
                CustomerId = order.CustomerId,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    VariantId = od.VariantId,
                    ProductName = variants[od.VariantId].Product.Name,
                    VariantImage = variants[od.VariantId].Product.DetailImage?.Image1,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice, 
                    Subtotal = od.SubTotal    
                }).ToList()
            };
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

            // FIX 4: Update chuỗi Include (Thêm Variant vào giữa)
            var items = await _repository.GetWithPaginationAsync(pageInfo, filter, "OrderDetails.Variant.Product.DetailImage,Customer.User");

            var totalItems = await _repository.CountAsync(filter);
            double totalPrice = 0;

            if (searchCondition.isTotalPrice.HasValue && searchCondition.isTotalPrice.Value)
            {
                totalPrice = items.Sum(o => o.TotalPrice);
            }

            // FIX 5: Cập nhật Mapster config để trỏ đúng đường dẫn
            TypeAdapterConfig<OrderDetail, OrderDetailDTO>.NewConfig()
                    .Map(dest => dest.ProductName, src => src.Variant.Product.Name)
                    .Map(dest => dest.VariantImage, src => src.Variant.Product.DetailImage != null ? src.Variant.Product.DetailImage.Image1 : null);

            TypeAdapterConfig<Customer, CustomerDTO>.NewConfig()
                  .Map(dest => dest.Fullname, src => src.FullName)
                  .Map(dest => dest.Phone, src => src.Phone)
                  .Map(dest => dest.CustomerAddress, src => src.Address)
                  .Map(dest => dest.Email, src => src.User.Email);

            var ordersDto = items.Adapt<IEnumerable<OrderDTO>>();

            return (ordersDto, totalPrice, totalItems);
        }

        public async Task<OrderDTO> Get(int id)
        {
            // FIX 6: Update chuỗi Include giống như GetAll
            var item = await _repository.GetAsync(
             id, "OrderDetails.Variant.Product.DetailImage,Customer.User"
            ) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            TypeAdapterConfig<OrderDetail, OrderDetailDTO>.NewConfig()
                .Map(dest => dest.ProductName, src => src.Variant.Product.Name)
                .Map(dest => dest.VariantImage, src => src.Variant.Product.DetailImage != null ? src.Variant.Product.DetailImage.Image1 : null);

            TypeAdapterConfig<Customer, CustomerDTO>.NewConfig()
              .Map(dest => dest.Fullname, src => src.FullName)
              .Map(dest => dest.Phone, src => src.Phone)
              .Map(dest => dest.CustomerAddress, src => src.Address)
              .Map(dest => dest.Email, src => src.User.Email);

            return item.Adapt<OrderDTO>();
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
    }
}
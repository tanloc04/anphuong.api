using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Orders;

namespace anphuong.Core.Interfaces.Services
{
    public interface IOrderService
    {
        public Task<OrderDTO> PlaceOrderAsync(CreateOrderRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<OrderDTO>, int totalItems)> GetAll(SearchOrderRequestDTO request);

        public Task<OrderDTO> Get(int id);

        //public Task<OrderDTO> Update(int id, UpdateOrderRequestDTO request);
    }
}

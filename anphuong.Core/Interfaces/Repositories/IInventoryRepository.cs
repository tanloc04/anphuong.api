using anphuong.Core.Domains.DTOs.RequestDTOs.Orders;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Repositories
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task CheckStockAsync(IEnumerable<OrderItemQuantity> items);
        Task ReduceStockBatchAsync(IEnumerable<OrderItemQuantity> items);
    }
}

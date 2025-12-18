using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task AddRangeOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails);
    }
}

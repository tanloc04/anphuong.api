using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;

namespace anphuong.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly anphuongDbContext _context;

        public OrderRepository(anphuongDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                return;

            await _context.Set<OrderDetail>().AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();
        }
    }
}

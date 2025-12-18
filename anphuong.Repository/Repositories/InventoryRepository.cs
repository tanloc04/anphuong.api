using anphuong.Core.Domains.DTOs.RequestDTOs.Orders;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace anphuong.Repository.Repositories
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        private readonly anphuongDbContext _context;

        public InventoryRepository(anphuongDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task CheckStockAsync(IEnumerable<OrderItemQuantity> items)
        {
            var productIds = items.Select(x => x.ProductId).ToList();

            var inventories = await _context.Inventories
                .Where(i => productIds.Contains(i.ProductId) && !i.IsDeleted)
                .ToListAsync();

            foreach (var item in items)
            {
                var inventory = inventories.FirstOrDefault(i => i.ProductId == item.ProductId);

                if (inventory == null)
                    throw new BusinessException(ErrorDetails.OUT_OF_STOCK);

                if (inventory.QuantityInStock < item.Quantity)
                    throw new BusinessException(ErrorDetails.OUT_OF_STOCK);
            }
        }
        public async Task ReduceStockBatchAsync(IEnumerable<OrderItemQuantity> items)
        {
            var productIds = items.Select(x => x.ProductId).ToList();

            var inventories = await _context.Inventories
                .Where(i => productIds.Contains(i.ProductId) && !i.IsDeleted)
                .ToListAsync();

            foreach (var item in items)
            {
                var inventory = inventories.First(i => i.ProductId == item.ProductId);
                inventory.QuantityInStock -= item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

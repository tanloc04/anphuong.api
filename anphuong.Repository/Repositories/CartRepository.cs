using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Repository.Repositories
{
    public class CartRepository: GenericRepository<Cart>, ICartRepository
    {
        private readonly anphuongDbContext _context;
        public CartRepository(anphuongDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task <Cart> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts.Include(c => c.CartItems.Where(ci => !ci.IsDeleted)).ThenInclude(ci => ci.Product).ThenInclude(p => p.DetailImage).Include(c => c.CartItems).ThenInclude(ci => ci.Variant).ThenInclude(v => v.Color).Include(c => c.CartItems).ThenInclude(ci => ci.Variant).ThenInclude(v => v.Material).Include(c => c.CartItems).ThenInclude(ci => ci.Variant).ThenInclude(v => v.Inventory).FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }
    }
}

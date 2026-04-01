using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Cart;
using anphuong.Core.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services
{
    public interface ICartService
    {
        Task SyncCartAsync(int userId, List<SyncCartRequestDTO> localItems);
        Task<CartDTO> GetCartAsync(int userId);
        Task ClearCartAsync(int userId);
        Task AddToCartAsync(int userId, SyncCartRequestDTO request);
        Task UpdateCartItemAsync(int userId, int cartItemId, int quantity);
        Task RemoveCartItemAsync(int userId, int cartItemId);
    }
}

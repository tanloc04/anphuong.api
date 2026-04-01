using anphuong.Core.Domains.DTOs.RequestDTOs.Cart;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("Id")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncCart([FromBody] List<SyncCartRequestDTO> localItems)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { success = false, message = "Không xác thực được người dùng!" });

                await _service.SyncCartAsync(userId, localItems);
                return Ok(new { success = true, message = "Đồng bộ giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi đồng bộ giỏ hàng: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { success = false, message = "Không xác thực được người dùng!" });

                var cart = await _service.GetCartAsync(userId);
                return Ok(new { success = true, data = cart });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] SyncCartRequestDTO request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { success = false, message = "Không xác thực được người dùng!" });

                await _service.AddToCartAsync(userId, request);
                return Ok(new { success = true, message = "Đã thêm vào giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        
        [HttpPut("update/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] int quantity)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { success = false, message = "Không xác thực được người dùng!" });

                await _service.UpdateCartItemAsync(userId, cartItemId, quantity);
                return Ok(new { success = true, message = "Cập nhật số lượng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { success = false, message = "Không xác thực được người dùng!" });

                await _service.RemoveCartItemAsync(userId, cartItemId);
                return Ok(new { success = true, message = "Đã xóa khỏi giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
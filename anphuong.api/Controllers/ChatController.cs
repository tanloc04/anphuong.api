using anphuong.Repository.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly anphuongDbContext _context;

        public ChatController(anphuongDbContext context)
        {
            _context = context;
        }

        // GET: api/chat/history/{userId}
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetChatHistory(int userId)
        {
            try
            {
                // Lấy toàn bộ tin nhắn của userId này, sắp xếp theo thời gian cũ -> mới
                var messages = await _context.ChatMessages
                    .Where(m => m.UserId == userId)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // 1. SỬA LẠI API NÀY ĐỂ LẤY SỐ TIN CHƯA ĐỌC
        [HttpGet("users")]
        public async Task<IActionResult> GetChatUsers()
        {
            try
            {
                var chatUsers = await _context.ChatMessages
                    .GroupBy(m => m.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        LastMessageTime = g.Max(m => m.Timestamp),
                        // 👇 Thêm dòng này: Đếm số tin nhắn của User gửi mà Admin chưa đọc
                        UnreadCount = g.Count(m => m.Sender == "User" && !m.IsRead)
                    })
                    .OrderByDescending(u => u.LastMessageTime)
                    .ToListAsync();

                return Ok(new { success = true, data = chatUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // 2. THÊM API NÀY ĐỂ ĐÁNH DẤU ĐÃ ĐỌC TIN NHẮN
        [HttpPost("mark-read/{userId}")]
        public async Task<IActionResult> MarkRead(int userId, [FromQuery] string role)
        {
            try
            {
                // Nếu role = Admin -> Admin đang đọc tin của User (Đổi IsRead của Sender="User" thành true)
                string senderToMark = role == "Admin" ? "User" : "Admin";

                var unreadMsgs = await _context.ChatMessages
                    .Where(m => m.UserId == userId && m.Sender == senderToMark && !m.IsRead)
                    .ToListAsync();

                foreach (var msg in unreadMsgs)
                {
                    msg.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

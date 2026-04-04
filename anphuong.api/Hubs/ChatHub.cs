using anphuong.Core.Domains.Entities;
using anphuong.Repository.Context;
using Microsoft.AspNetCore.SignalR;

namespace anphuong.api.Hubs
{
    public class ChatHub: Hub
    {
        private readonly anphuongDbContext _context; // Nếu sếp dùng Repository thì thay bằng IChatRepository nhé

        public ChatHub(anphuongDbContext context)
        {
            _context = context;
        }

        // Khách gửi cho Admin
        public async Task SendMessageToAdmin(int userId, string message)
        {
            // 1. Lưu vào DB trước
            var chatMsg = new ChatMessage
            {
                UserId = userId,
                Sender = "User",
                Content = message,
                Timestamp = DateTime.Now,
                IsRead = false
            };
            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // 2. Bắn qua SignalR cho Admin thấy
            await Clients.Group("Admins").SendAsync("ReceiveMessage", chatMsg);
        }

        // Admin rep lại cho Khách
        public async Task SendMessageToUser(int userId, string message)
        {
            // 1. Lưu vào DB
            var chatMsg = new ChatMessage
            {
                UserId = userId,
                Sender = "Admin",
                Content = message,
                Timestamp = DateTime.Now,
                IsRead = false
            };
            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // 2. Bắn về phòng riêng của Khách
            await Clients.Group($"User_{userId}").SendAsync("ReceiveMessage", chatMsg);
        }

        public async Task JoinUserRoom(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinAdminRoom()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}

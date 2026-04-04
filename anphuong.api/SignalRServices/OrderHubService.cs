using anphuong.api.Hubs;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace anphuong.api.SignalRServices
{
    public class OrderHubService: IOrderHubService
    {
        private readonly IHubContext<OrderHub> _hubContext;
        public OrderHubService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewOrderAsync()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder");
        }
    }
}

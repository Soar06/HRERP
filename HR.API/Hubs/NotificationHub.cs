using Microsoft.AspNetCore.SignalR;

namespace HR.API.Hubs;

public class NotificationHub : Hub
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHub(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotification(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task NotifyLeaveRequest(int leaveRequestId)
    {
        await _hubContext.Clients.All.SendAsync("LeaveRequestUpdated", leaveRequestId);
    }
}
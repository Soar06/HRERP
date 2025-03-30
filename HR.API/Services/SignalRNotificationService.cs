using HR.API.Hubs;
using HR.Core.Interfaces;
using HR.Core.Services;
using Microsoft.AspNetCore.SignalR;

namespace HR.API.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAdminsAsync(string message, int requestId)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", message, requestId);
    }
}
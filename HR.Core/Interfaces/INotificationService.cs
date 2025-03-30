namespace HR.Core.Interfaces;

public interface INotificationService
{
    Task NotifyAdminsAsync(string message, int requestId);
}
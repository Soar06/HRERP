using HR.Core.Entities;
using HR.Core.Interfaces;

namespace HR.Core.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _repository;
    private readonly INotificationService _notificationService;

    public LeaveRequestService(ILeaveRequestRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest, int employeeId)
    {
        leaveRequest.EmployeeId = employeeId;
        var createdRequest = await _repository.AddAsync(leaveRequest);

        // Notify admins
        await _notificationService.NotifyAdminsAsync("New leave request submitted", createdRequest.Id);

        return createdRequest;
    }

    public async Task<List<LeaveRequest>> GetMyRequestsAsync(int employeeId)
    {
        return await _repository.GetByEmployeeIdAsync(employeeId);
    }

    public async Task<List<LeaveRequest>> GetPendingRequestsAsync()
    {
        return await _repository.GetPendingAsync();
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        var leaveRequest = await _repository.GetByIdAsync(id);
        leaveRequest.Status = Enum.Parse<LeaveStatus>(status, true);
        await _repository.UpdateAsync(leaveRequest);
    }
}
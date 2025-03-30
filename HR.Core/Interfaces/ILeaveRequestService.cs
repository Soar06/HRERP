using HR.Core.Entities;

namespace HR.Core.Services;

public interface ILeaveRequestService
{
    Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest, int employeeId);
    Task<List<LeaveRequest>> GetMyRequestsAsync(int employeeId);
    Task<List<LeaveRequest>> GetPendingRequestsAsync();
    Task UpdateStatusAsync(int id, string status);
}
using HR.Core.Entities;

namespace HR.Core.Interfaces;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<List<LeaveRequest>> GetPendingAsync();
    Task<LeaveRequest> GetByIdAsync(int id);
    Task UpdateAsync(LeaveRequest leaveRequest);
}
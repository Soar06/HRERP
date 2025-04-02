using HR.Core.Entities;
using HR.Core.Interfaces;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _context;

    public LeaveRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<List<LeaveRequest>> GetPendingAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == LeaveStatus.Pending)
            .ToListAsync();
    }

    public async Task<LeaveRequest> GetByIdAsync(int id)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id) ?? throw new Exception("Leave request not found");
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync();
    }
}
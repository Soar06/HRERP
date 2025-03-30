namespace HR.Core.Entities;

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
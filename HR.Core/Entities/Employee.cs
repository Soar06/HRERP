namespace HR.Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public List<LeaveRequest> LeaveRequests { get; set; } = new();
}
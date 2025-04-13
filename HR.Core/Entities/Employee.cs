namespace HR.Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty; // Add PhoneNumber field
    public string ContractLink { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string Level { get; set; } = "Fresher";
    public int? Ratings { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public List<LeaveRequest> LeaveRequests { get; set; } = new();
}
namespace HR.Core.DTOs.EmployeeDTOs;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
}
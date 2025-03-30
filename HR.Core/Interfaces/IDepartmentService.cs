using HR.Core.Entities;

namespace HR.Core.Services;

public interface IDepartmentService
{
    Task<List<Department>> GetAllAsync();
    Task<Department> CreateAsync(Department department);
}
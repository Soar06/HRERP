using HR.Core.Entities;

namespace HR.Core.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<Department> AddAsync(Department department);
}
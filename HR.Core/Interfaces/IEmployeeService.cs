using HR.Core.Entities;

namespace HR.Core.Services;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync(int page, int pageSize);
    Task<Employee> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(int id, Employee employee);
    Task DeleteAsync(int id);
}
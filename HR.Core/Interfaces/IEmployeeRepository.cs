using HR.Core.Entities;

namespace HR.Core.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(int page, int pageSize);
    Task<Employee> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}

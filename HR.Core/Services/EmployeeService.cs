using HR.Core.Entities;
using HR.Core.Interfaces;

namespace HR.Core.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Employee>> GetAllAsync(int page, int pageSize)
    {
        return await _repository.GetAllAsync(page, pageSize);
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        return await _repository.AddAsync(employee);
    }

    public async Task UpdateAsync(int id, Employee employee)
    {
        var existingEmployee = await _repository.GetByIdAsync(id);
        existingEmployee.Name = employee.Name;
        existingEmployee.Email = employee.Email;
        existingEmployee.DepartmentId = employee.DepartmentId;
        await _repository.UpdateAsync(existingEmployee);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
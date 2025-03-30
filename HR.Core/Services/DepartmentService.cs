using HR.Core.Entities;
using HR.Core.Interfaces;

namespace HR.Core.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Department> CreateAsync(Department department)
    {
        return await _repository.AddAsync(department);
    }

}
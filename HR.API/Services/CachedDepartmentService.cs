using HR.Core.Entities;
using HR.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace HR.API.Services;

public class CachedDepartmentService : IDepartmentService
{
    private readonly IDepartmentService _departmentService;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "departments";

    public CachedDepartmentService(IDepartmentService departmentService, IMemoryCache cache)
    {
        _departmentService = departmentService;
        _cache = cache;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            return await _departmentService.GetAllAsync();
        });
    }

    public async Task<Department> CreateAsync(Department department)
    {
        var createdDepartment = await _departmentService.CreateAsync(department);
        _cache.Remove(CacheKey); // Invalidate cache
        return createdDepartment;
    }
}
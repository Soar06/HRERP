using HR.Core.Entities;
using HR.Core.Interfaces;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("Employee not found");
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await GetByIdAsync(id);
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
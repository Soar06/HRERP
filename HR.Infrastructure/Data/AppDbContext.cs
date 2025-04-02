using HR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId);

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@hr.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            }
        );
    }
}
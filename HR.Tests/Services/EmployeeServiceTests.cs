using HR.Core.Entities;
using HR.Core.Interfaces;
using HR.Core.Services;
using Moq;


namespace HR.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepo;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepo = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "John", Email = "john@hr.com" };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task CreateAsync_CallsRepository()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "Jane", Email = "jane@hr.com", DepartmentId = 1 };
        _mockRepo.Setup(repo => repo.AddAsync(employee)).ReturnsAsync(employee);

        // Act
        var result = await _service.CreateAsync(employee);

        // Assert
        _mockRepo.Verify(repo => repo.AddAsync(employee), Times.Once());
        Assert.Equal(employee, result);
    }
}
using HR.Core.Entities;
using HR.Core.Interfaces;
using HR.Core.Services;
using HR.Core.DTOs.AuthDTOs;
using Moq;
using Xunit;

namespace HR.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _service = new AuthService(_mockRepo.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "admin@hr.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin" };
        var request = new LoginRequest { Email = "admin@hr.com", Password = "admin123" };
        _mockRepo.Setup(repo => repo.GetByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(request);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var request = new LoginRequest { Email = "admin@hr.com", Password = "wrong" };
        _mockRepo.Setup(repo => repo.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AuthenticateAsync(request));
    }
}
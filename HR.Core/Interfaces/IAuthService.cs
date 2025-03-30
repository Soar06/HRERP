using HR.Core.DTOs.AuthDTOs;
using HR.Core.Entities;

namespace HR.Core.Services;

public interface IAuthService
{
    Task<User> AuthenticateAsync(LoginRequest request);
    Task RegisterAsync(string email, string password, string role);
}
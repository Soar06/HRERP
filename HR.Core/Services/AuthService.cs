using HR.Core.DTOs.AuthDTOs;
using HR.Core.Entities;
using HR.Core.Interfaces;

namespace HR.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AuthenticateAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return user;
    }

    public async Task RegisterAsync(string email, string password, string role)
    {
        if (await _userRepository.ExistsByEmailAsync(email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
        await _userRepository.AddAsync(user);
    }
}
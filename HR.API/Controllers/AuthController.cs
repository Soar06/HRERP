using HR.Core.DTOs.AuthDTOs;
using HR.Core.Services;
using HR.Core.Interfaces; // Add this for IJwtTokenService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService; 

    public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")] 
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.AuthenticateAsync(request);
        var token = _jwtTokenService.GenerateToken(user.Email, user.Role, user.Id); 
        return Ok(new AuthResponse { Token = token });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] string email, string password, string role = "Employee")
    {
        await _authService.RegisterAsync(email, password, role);
        return Ok("User registered");
    }
}
namespace HR.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string email, string role, int userId);
}
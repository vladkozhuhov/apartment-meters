using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string username, IEnumerable<string> roles);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        string? ValidateToken(string token);
    }
} 
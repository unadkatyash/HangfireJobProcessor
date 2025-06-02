using System.Security.Claims;

namespace HangfireJobProcessor.IService
{
    public interface IJwtService
    {
        string GenerateToken(string username, string[] roles);
        bool ValidateToken(string token);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}

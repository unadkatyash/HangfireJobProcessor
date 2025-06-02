using System.Security.Claims;

namespace HangfireJobProcessor.IService
{
    public interface IJwtService
    {
        #region JWT Token Management

        /// <summary>
        /// Generates a JWT token for the specified username and roles.
        /// </summary>
        /// <param name="username">The username for whom the token is generated.</param>
        /// <param name="roles">Array of roles to include in the token claims.</param>
        /// <returns>A JWT token string.</returns>
        string GenerateToken(string username, string[] roles);

        /// <summary>
        /// Validates the given JWT token for authenticity and expiry.
        /// </summary>
        /// <param name="token">The JWT token string to validate.</param>
        /// <returns>True if token is valid; otherwise, false.</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Extracts the ClaimsPrincipal from a valid JWT token.
        /// </summary>
        /// <param name="token">The JWT token string.</param>
        /// <returns>ClaimsPrincipal representing the token's claims.</returns>
        ClaimsPrincipal GetPrincipalFromToken(string token);

        #endregion
    }
}

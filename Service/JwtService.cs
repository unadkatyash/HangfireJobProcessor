using HangfireJobProcessor.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HangfireJobProcessor.Service
{
    /// <summary>
    /// Service for generating and validating JSON Web Tokens (JWT).
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Generates a JWT for the specified user and roles.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="roles">The user roles.</param>
        /// <returns>JWT as a string.</returns>
        public string GenerateToken(string username, string[] roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim("username", username)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Hangfire dashboard access permission for Admins
            if (roles.Contains("Admin"))
            {
                claims.Add(new Claim("permission", "hangfire-dashboard"));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            _logger.LogInformation("JWT generated for user {Username} with roles: {Roles}", username, string.Join(", ", roles));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates the provided JWT.
        /// </summary>
        /// <param name="token">JWT token string.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Extracts the ClaimsPrincipal from a JWT if valid.
        /// </summary>
        /// <param name="token">JWT token string.</param>
        /// <returns>ClaimsPrincipal or empty if invalid.</returns>
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to extract claims from token: {Message}", ex.Message);
                return new ClaimsPrincipal();
            }
        }
    }
}


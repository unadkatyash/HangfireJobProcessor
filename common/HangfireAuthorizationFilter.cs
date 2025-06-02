using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HangfireJobProcessor.common
{
    public class HangfireJwtAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var logger = httpContext.RequestServices.GetService<ILogger<HangfireJwtAuthorizationFilter>>();

            // 1. Allow localhost in development
            //if (IsLocalhost(httpContext))
            //{
            //    logger?.LogInformation("Localhost access granted to Hangfire dashboard");
            //    return true;
            //}

            // 2. Check JWT Bearer token
            string token = null;

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            // 2. Fallback to cookie
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.Request.Cookies["HangfireToken"];
            }

            if (!string.IsNullOrEmpty(token))
            {
                if (ValidateJwtToken(token, configuration, out ClaimsPrincipal principal))
                {
                    httpContext.User = principal;

                    if (principal.IsInRole("Admin") ||
                        principal.IsInRole("HangfireAdmin") ||
                        principal.HasClaim("permission", "hangfire-dashboard"))
                    {
                        var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                        logger?.LogInformation("JWT authentication successful for user: {Username}", username);
                        return true;
                    }
                    else
                    {
                        logger?.LogWarning("User authenticated but lacks Hangfire permissions: {Username}",
                            principal.FindFirst(ClaimTypes.Name)?.Value);
                    }
                }
            }

            logger?.LogWarning("Unauthorized access attempt to Hangfire dashboard from {RemoteIP}",
                httpContext.Connection.RemoteIpAddress?.ToString());

            return false;
        }

        private bool IsLocalhost(HttpContext httpContext)
        {
            var host = httpContext.Request.Host.Host;
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();

            return host.Contains("localhost") ||
                   host == "127.0.0.1" ||
                   remoteIp == "::1" ||
                   remoteIp == "127.0.0.1";
        }

        private bool ValidateJwtToken(string token, IConfiguration configuration, out ClaimsPrincipal principal)
        {
            principal = new ClaimsPrincipal();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

                principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HangfireJobProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Services & Constructor

        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtService jwtService, IUserService userService, ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _userService = userService;
            _logger = logger;
        }

        #endregion

        #region Auth Summary

        // This region can later include endpoints or logic for summarizing login activities
        // such as: login attempts, active sessions, failed login counts, etc.

        #endregion

        #region Auth Endpoints

        /// <summary>
        /// Authenticates a user with the provided credentials and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request containing username and password.</param>
        /// <returns>Returns a JWT token if authentication is successful; otherwise, returns Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Username and password are required" });
                }

                var user = await _userService.AuthenticateAsync(request.Username, request.Password);

                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = _jwtService.GenerateToken(user.Username, user.Roles);

                Response.Cookies.Append("HangfireToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                _logger.LogInformation("Successful login for user: {Username}", user.Username);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    Roles = user.Roles,
                    ExpiresAt = DateTime.UtcNow.AddHours(8)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Validates the provided JWT token.
        /// </summary>
        /// <param name="token">JWT token string to validate.</param>
        /// <returns>Returns token validation result including username and roles if valid.</returns>
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Token is required" });

                var isValid = _jwtService.ValidateToken(token);

                if (isValid)
                {
                    var principal = _jwtService.GetPrincipalFromToken(token);
                    var username = principal.FindFirst(ClaimTypes.Name)?.Value;

                    return Ok(new
                    {
                        valid = true,
                        username = username,
                        roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
                    });
                }

                return Ok(new { valid = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Returns the authenticated user's profile information.
        /// </summary>
        /// <returns>Returns the username, roles, and permission to access Hangfire dashboard.</returns>
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            return Ok(new
            {
                username = username,
                roles = roles,
                canAccessHangfire = User.HasClaim("permission", "hangfire-dashboard") || User.IsInRole("Admin")
            });
        }

        #endregion
    }
}

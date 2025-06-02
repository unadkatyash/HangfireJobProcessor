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
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtService jwtService, IUserService userService, ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _userService = userService;
            _logger = logger;
        }

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
    }
}
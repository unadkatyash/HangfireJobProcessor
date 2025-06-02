using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;
using System.Security.Cryptography;
using System.Text;

namespace HangfireJobProcessor.Service
{
    /// <summary>
    /// In-memory user service for authentication and user lookup.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly List<User> _users;
        private readonly ILogger<UserService>? _logger;
        private const string Salt = "fixed-salt-value";

        /// <summary>
        /// Initializes the in-memory user list.
        /// </summary>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public UserService(ILogger<UserService>? logger = null)
        {
            _logger = logger;

            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Roles = new[] { "Admin", "HangfireAdmin" },
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "hangfire_user",
                    PasswordHash = HashPassword("hangfire123"),
                    Roles = new[] { "HangfireUser" },
                    IsActive = true
                }
            };
        }

        /// <inheritdoc />
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);

            if (user == null || !user.IsActive)
            {
                _logger?.LogWarning("Authentication failed for inactive or missing user: {Username}", username);
                return null;
            }

            if (VerifyPassword(password, user.PasswordHash))
            {
                _logger?.LogInformation("User {Username} authenticated successfully.", username);
                return user;
            }

            _logger?.LogWarning("Invalid password for user: {Username}", username);
            return null;
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Hashes a password using SHA256 with a static salt.
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + Salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies the provided password against the stored hash.
        /// </summary>
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}


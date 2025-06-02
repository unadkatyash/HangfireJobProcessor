using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;
using System.Security.Cryptography;
using System.Text;

namespace HangfireJobProcessor.Service
{
    public class UserService : IUserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"), // Default password
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

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);

            if (user == null || !user.IsActive)
                return null;

            if (VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}

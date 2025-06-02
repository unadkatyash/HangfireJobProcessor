namespace HangfireJobProcessor.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public DateTime ExpiresAt { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public bool IsActive { get; set; } = true;
    }
}

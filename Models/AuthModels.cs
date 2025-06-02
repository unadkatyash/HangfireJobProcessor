namespace HangfireJobProcessor.Models
{
    #region Authentication Models

    /// <summary>
    /// Represents a login request with username and password.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username for login.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for login.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the response returned after a successful login.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the JWT token issued on login.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the logged-in user.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public string[] Roles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the expiration date and time of the token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }

    #endregion

    #region User Model

    /// <summary>
    /// Represents a system user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed password.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public string[] Roles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    #endregion
}

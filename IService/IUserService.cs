using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.IService
{
    public interface IUserService
    {
        #region User Authentication and Retrieval

        /// <summary>
        /// Authenticates a user asynchronously using username and password.
        /// </summary>
        /// <param name="username">The username of the user to authenticate.</param>
        /// <param name="password">The password of the user to authenticate.</param>
        /// <returns>The authenticated User object if successful; otherwise, null.</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Retrieves a user asynchronously by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The User object if found; otherwise, null.</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        #endregion
    }
}

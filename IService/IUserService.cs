using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.IService
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}

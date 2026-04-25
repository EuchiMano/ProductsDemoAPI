using NewProjectFromScratch.Domain.Entities;

namespace NewProjectFromScratch.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateCredentialsAsync(string username, string password);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateUserAsync(string username, string password, string role);
        Task<User> UpdateUserAsync(string username, string? password, string? role);
        Task DeleteUserAsync(string username);
    }
}

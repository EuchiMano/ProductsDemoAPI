using System.Linq;
using NewProjectFromScratch.Application.Interfaces;
using NewProjectFromScratch.Domain.Entities;

namespace NewProjectFromScratch.Infrastructure.Security
{
    public sealed class InMemoryUserService : IUserService
    {
        private readonly List<User> _users = new()
        {
            new User("user", "test123", "User"),
            new User("admin", "test123", "Admin")
        };

        public Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(x =>
                string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User> CreateUserAsync(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Role is required.", nameof(role));
            }

            if (_users.Any(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Username already exists.", nameof(username));
            }

            var user = new User(username.Trim(), password, role.Trim());
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> UpdateUserAsync(string username, string? password, string? role)
        {
            var userIndex = _users.FindIndex(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
            if (userIndex < 0)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var existingUser = _users[userIndex];
            var updatedUser = new User(
                existingUser.Username,
                string.IsNullOrWhiteSpace(password) ? existingUser.Password : password,
                string.IsNullOrWhiteSpace(role) ? existingUser.Role : role.Trim());

            _users[userIndex] = updatedUser;
            return Task.FromResult(updatedUser);
        }

        public Task DeleteUserAsync(string username)
        {
            var user = _users.FirstOrDefault(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
            if (user is null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _users.Remove(user);
            return Task.CompletedTask;
        }
    }
}

using NewProjectFromScratch.Infrastructure.Security;
using Xunit;

namespace NewProjectFromScratch.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task ValidateCredentialsAsync_ShouldReturnUserForValidCredentials()
        {
            var service = new InMemoryUserService();

            var user = await service.ValidateCredentialsAsync("admin", "test123");

            Assert.NotNull(user);
            Assert.Equal("admin", user!.Username);
            Assert.Equal("Admin", user.Role);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnCreatedUser()
        {
            var service = new InMemoryUserService();

            var user = await service.CreateUserAsync("newuser", "password", "User");

            Assert.NotNull(user);
            Assert.Equal("newuser", user.Username);
            Assert.Equal("User", user.Role);

            var retrieved = await service.GetByUsernameAsync("newuser");
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUser()
        {
            var service = new InMemoryUserService();
            await service.CreateUserAsync("deleteme", "pass", "User");

            await service.DeleteUserAsync("deleteme");
            var removed = await service.GetByUsernameAsync("deleteme");

            Assert.Null(removed);
        }
    }
}

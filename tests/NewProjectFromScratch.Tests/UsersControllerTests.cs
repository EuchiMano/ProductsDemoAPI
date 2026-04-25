using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Controllers;
using NewProjectFromScratch.Infrastructure.Security;
using Xunit;

namespace NewProjectFromScratch.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetUsers_ShouldReturnSeededUsers()
        {
            var userService = new InMemoryUserService();
            var controller = new UsersController(userService);

            var result = await controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);

            Assert.Contains(users, x => x.Username == "admin");
            Assert.Contains(users, x => x.Username == "user");
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            var userService = new InMemoryUserService();
            var controller = new UsersController(userService);

            var request = new CreateUserRequest("testuser", "password", "User");
            var result = await controller.CreateUser(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var user = Assert.IsType<UserDto>(createdResult.Value);

            Assert.Equal("testuser", user.Username);
            Assert.Equal("User", user.Role);
        }
    }
}

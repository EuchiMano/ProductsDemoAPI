using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Controllers;
using NewProjectFromScratch.Infrastructure.Security;
using Xunit;

namespace NewProjectFromScratch.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_ShouldReturnJwtTokenForValidCredentials()
        {
            var userService = new InMemoryUserService();
            var jwtSettings = Options.Create(new JwtSettings
            {
                Key = "ThisIsASecretKeyForTestingAtLeast32Chars",
                Issuer = "NewProjectFromScratch",
                Audience = "NewProjectFromScratchUsers",
                ExpiryMinutes = 60
            });

            var controller = new AuthController(userService, jwtSettings);
            var result = await controller.Login(new LoginRequest("admin", "test123"));

            var okResult = Assert.IsType<OkObjectResult>(result);
            var authResponse = Assert.IsType<AuthResponse>(okResult.Value);

            Assert.Equal("admin", authResponse.Username);
            Assert.Equal("Admin", authResponse.Role);
            Assert.False(string.IsNullOrWhiteSpace(authResponse.Token));
        }
    }
}

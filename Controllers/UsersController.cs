using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Application.Interfaces;

namespace NewProjectFromScratch.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "Administrators")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users.Select(x => new UserDto(x.Username, x.Role)));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userService.GetByUsernameAsync(username);
            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new UserDto(user.Username, user.Role));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request.Username, request.Password, request.Role);
                return CreatedAtAction(nameof(GetUser), new { username = user.Username }, new UserDto(user.Username, user.Role));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(username, request.Password, request.Role);
                return Ok(new UserDto(user.Username, user.Role));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            try
            {
                await _userService.DeleteUserAsync(username);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}

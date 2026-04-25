namespace NewProjectFromScratch.Application.DTOs
{
    public sealed record UserDto(string Username, string Role);
    public sealed record CreateUserRequest(string Username, string Password, string Role);
    public sealed record UpdateUserRequest(string? Password, string? Role);
    public sealed record LoginRequest(string Username, string Password);
    public sealed record AuthResponse(string Token, string Username, string Role);
}

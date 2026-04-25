namespace NewProjectFromScratch.Domain.Entities
{
    public sealed class User
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public string Role { get; init; }

        public User(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}

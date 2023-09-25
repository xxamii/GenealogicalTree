namespace Core.Models
{
    public class User : Entity
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
    }
}

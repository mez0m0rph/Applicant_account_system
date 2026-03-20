public class User
{
    public Guid Id { get; set; } = !null;
    public string Email { get; set; }
    public string PasswordHash { get; set; } = !null;
    public string Role { get; set; } = !null;
}
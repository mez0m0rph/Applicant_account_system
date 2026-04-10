using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }
}

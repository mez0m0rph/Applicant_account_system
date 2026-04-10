namespace AuthService.Application.DTOs;

public class CurrentUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}

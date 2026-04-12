namespace ManagerService.Application.DTOs;

public class ManagerResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Faculty { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
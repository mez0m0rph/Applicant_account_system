namespace ManagerService.Domain.Entities;

public class Manager
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty; // Manager / MainManager
    public string Faculty { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

using ManagerService.Domain.Enums;

namespace ManagerService.Domain.Entities;

public class Manager
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public ManagerRole Role { get; set; }
    public string Faculty { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
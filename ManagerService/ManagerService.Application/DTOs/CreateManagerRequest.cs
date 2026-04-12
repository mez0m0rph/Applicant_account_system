using ManagerService.Domain.Enums;

namespace ManagerService.Application.DTOs;

public class CreateManagerRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public ManagerRole Role { get; set; }
    public string Faculty { get; set; } = null!;
}
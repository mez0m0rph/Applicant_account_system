namespace ManagerService.Application.DTOs;

public class UpdateManagerRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
}

namespace AdmissionService.Application.DTOs.External;

public class ManagerCatalogItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

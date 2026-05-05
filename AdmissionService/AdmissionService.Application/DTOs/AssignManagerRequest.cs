namespace AdmissionService.Application.DTOs;

public class AssignManagerRequest
{
    public Guid ManagerUserId { get; set; }
    public string ManagerEmail { get; set; } = string.Empty;
}

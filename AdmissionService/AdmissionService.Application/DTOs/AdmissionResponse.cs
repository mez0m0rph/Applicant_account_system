namespace AdmissionService.Application.DTOs;

public class AdmissionResponse
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ApplicantFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? AssignedManagerUserId { get; set; }
    public string AssignedManagerName { get; set; } = string.Empty;
    public string AssignedManagerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<AdmissionProgramItemResponse> Programs { get; set; } = new();
}

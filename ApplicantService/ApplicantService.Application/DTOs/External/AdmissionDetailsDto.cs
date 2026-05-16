namespace ApplicantService.Application.DTOs.External;

public class AdmissionDetailsDto
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? AssignedManagerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
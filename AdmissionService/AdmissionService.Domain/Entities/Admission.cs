using AdmissionService.Domain.Enums;

namespace AdmissionService.Domain.Entities;

public class Admission
{
    public Guid Id { get; set; }

    public Guid ApplicantUserId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;

    public AdmissionStatus Status { get; set; }

    public Guid? AssignedManagerUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

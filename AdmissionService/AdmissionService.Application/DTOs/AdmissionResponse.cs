using AdmissionService.Domain.Enums;

namespace AdmissionService.Application.DTOs;

public class AdmissionResponse
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public AdmissionStatus Status { get; set; }
    public Guid? AssignedManagerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<AdmissionProgramDto> Programs { get; set; } = new();
}
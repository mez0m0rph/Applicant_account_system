namespace WebApp.Models.Admission;

public class AdmissionViewModel
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ApplicantFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? AssignedManagerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string AssignedManagerName { get; set; } = string.Empty;
    public string AssignedManagerEmail { get; set; } = string.Empty;

    public List<AdmissionProgramViewModel> Programs { get; set; } = new();
}

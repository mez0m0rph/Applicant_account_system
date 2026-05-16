namespace AdmissionService.Application.DTOs.External;

public class DocumentDetailsDto
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
    public string? EducationLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}

namespace DocumentService.Application.DTOs;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string Type { get; set; } = null!;
    public StoredFileDto File { get; set; } = null!;
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
    public DateTime CreatedAt { get; set; }
}
using DocumentService.Domain.Enums;

namespace DocumentService.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public DocumentType Type { get; set; }
    public Guid StoredFileId { get; set; }
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
    public DateTime CreatedAt { get; set; }
}
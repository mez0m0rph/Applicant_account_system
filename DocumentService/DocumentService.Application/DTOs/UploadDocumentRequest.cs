using DocumentService.Domain.Enums;

namespace DocumentService.Application.DTOs;

public class UploadDocumentRequest
{
    public DocumentType Type { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
}
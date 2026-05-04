using DocumentService.Domain.Enums;

namespace DocumentService.Application.DTOs;

public class UploadDocumentRequest
{
    public DocumentType Type { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileContentBase64 { get; set; } = string.Empty;
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
}

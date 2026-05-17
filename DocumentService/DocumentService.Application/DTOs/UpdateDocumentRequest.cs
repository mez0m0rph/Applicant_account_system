using DocumentService.Domain.Enums;

namespace DocumentService.Application.DTOs;

public class UpdateDocumentRequest
{
    public DocumentType Type { get; set; }
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
    public string? EducationLevel { get; set; }
}

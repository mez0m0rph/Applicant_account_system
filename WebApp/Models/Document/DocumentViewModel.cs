namespace WebApp.Models.Document;

public class DocumentViewModel
{
    public Guid Id { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public StoredFileViewModel File { get; set; } = new();
    public string? SeriesNumber { get; set; }
    public string? IssuedBy { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? EducationDocumentName { get; set; }
    public DateTime CreatedAt { get; set; }
}

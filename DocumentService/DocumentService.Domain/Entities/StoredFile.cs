namespace DocumentService.Domain.Entities;

public class StoredFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}
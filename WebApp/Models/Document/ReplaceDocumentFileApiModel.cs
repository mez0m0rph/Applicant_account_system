namespace WebApp.Models.Document;

public class ReplaceDocumentFileApiModel
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileContentBase64 { get; set; } = string.Empty;
}

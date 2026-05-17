using Microsoft.AspNetCore.Http;

namespace WebApp.Models.Document;

public class ReplaceDocumentFileViewModel
{
    public Guid Id { get; set; }
    public IFormFile? UploadedFile { get; set; }
}

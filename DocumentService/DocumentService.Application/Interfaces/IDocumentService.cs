using DocumentService.Application.DTOs;

namespace DocumentService.Application.Interfaces;

public interface IDocumentService
{
    Task UploadAsync(Guid applicantUserId, string applicantEmail, UploadDocumentRequest request);
    Task<List<DocumentResponse>> GetMyDocumentsAsync(Guid applicantUserId);
}

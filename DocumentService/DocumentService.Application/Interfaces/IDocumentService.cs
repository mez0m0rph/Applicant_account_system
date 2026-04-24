using DocumentService.Application.DTOs;

namespace DocumentService.Application.Interfaces;

public interface IDocumentService
{
    Task UploadAsync(Guid applicantUserId, UploadDocumentRequest request);
    Task<List<DocumentResponse>> GetMyDocumentsAsync(Guid applicantUserId);
}
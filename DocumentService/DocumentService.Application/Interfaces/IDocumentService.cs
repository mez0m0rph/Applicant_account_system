using DocumentService.Application.DTOs;

namespace DocumentService.Application.Interfaces;

public interface IDocumentService
{
    Task UploadAsync(Guid applicantUserId, string applicantEmail, UploadDocumentRequest request);
    Task<List<DocumentResponse>> GetMyDocumentsAsync(Guid applicantUserId);
    Task<DownloadedFileDto> DownloadAsync(Guid applicantUserId, Guid documentId);
    Task DeleteAsync(Guid applicantUserId, Guid documentId);
    Task UpdateAsync(Guid applicantUserId, Guid documentId, UpdateDocumentRequest request);
    Task ReplaceFileAsync(Guid applicantUserId, Guid documentId, ReplaceDocumentFileRequest request);
}

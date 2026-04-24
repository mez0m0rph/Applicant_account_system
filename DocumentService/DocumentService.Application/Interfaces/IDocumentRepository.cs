using DocumentService.Domain.Entities;

namespace DocumentService.Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<List<Document>> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task AddDocumentAsync(Document document);
    Task AddStoredFileAsync(StoredFile file);
    Task<StoredFile?> GetStoredFileByIdAsync(Guid id);
}
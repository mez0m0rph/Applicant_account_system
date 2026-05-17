using DocumentService.Domain.Entities;

namespace DocumentService.Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<List<Document>> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task<StoredFile?> GetStoredFileByIdAsync(Guid id);

    Task AddStoredFileAsync(StoredFile file);
    Task AddDocumentAsync(Document document);
    Task UpdateDocumentAsync(Document document);

    Task DeleteDocumentAsync(Document document);
    Task DeleteStoredFileAsync(StoredFile file);
}

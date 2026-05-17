using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;
using Shared.Contracts.Events;
using Shared.Messaging.Interfaces;

namespace DocumentService.Infrastructure.Services;

public class DocumentServiceImpl : IDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAdmissionCatalogClient _admissionCatalogClient;

    public DocumentServiceImpl(
        IDocumentRepository repository,
        IMessagePublisher messagePublisher,
        IFileStorageService fileStorageService,
        IAdmissionCatalogClient admissionCatalogClient)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _fileStorageService = fileStorageService;
        _admissionCatalogClient = admissionCatalogClient;
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private async Task EnsureDocumentsEditableAsync(Guid applicantUserId)
    {
        var admission = await _admissionCatalogClient.GetByApplicantUserIdAsync(applicantUserId);

        if (admission != null &&
            string.Equals(admission.Status, "Closed", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Нельзя изменять документы, когда заявление закрыто");
        }
    }

    public async Task UploadAsync(Guid applicantUserId, string applicantEmail, UploadDocumentRequest request)
    {
        await EnsureDocumentsEditableAsync(applicantUserId);

        if (string.IsNullOrWhiteSpace(request.FileContentBase64))
            throw new Exception("Файл не передан");

        var fileBytes = Convert.FromBase64String(request.FileContentBase64);

        var storagePath = await _fileStorageService.UploadAsync(
            request.FileName,
            request.ContentType,
            fileBytes);

        var file = new StoredFile
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            ContentType = request.ContentType,
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow
        };

        await _repository.AddStoredFileAsync(file);

        var normalizedIssueDate = request.IssueDate.HasValue
            ? NormalizeUtc(request.IssueDate.Value)
            : DateTime.UtcNow;

        var document = new Document
        {
            Id = Guid.NewGuid(),
            ApplicantUserId = applicantUserId,
            Type = request.Type,
            StoredFileId = file.Id,
            SeriesNumber = request.SeriesNumber ?? string.Empty,
            IssuedBy = request.IssuedBy ?? string.Empty,
            BirthPlace = request.BirthPlace ?? string.Empty,
            IssueDate = normalizedIssueDate,
            EducationDocumentName = request.EducationDocumentName ?? string.Empty,
            EducationLevel = request.EducationLevel ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddDocumentAsync(document);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = applicantUserId,
            Email = applicantEmail,
            Subject = "Документ загружен",
            Message = $"Документ {request.FileName} успешно загружен."
        });
    }

    public async Task<List<DocumentResponse>> GetMyDocumentsAsync(Guid applicantUserId)
    {
        var documents = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        var result = new List<DocumentResponse>();

        foreach (var document in documents)
        {
            var file = await _repository.GetStoredFileByIdAsync(document.StoredFileId);

            if (file == null)
                throw new Exception("Файл документа не найден");

            result.Add(new DocumentResponse
            {
                Id = document.Id,
                ApplicantUserId = document.ApplicantUserId,
                Type = document.Type.ToString(),
                File = new StoredFileDto
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    StoragePath = file.StoragePath,
                    UploadedAt = file.UploadedAt
                },
                SeriesNumber = document.SeriesNumber ?? string.Empty,
                IssuedBy = document.IssuedBy ?? string.Empty,
                BirthPlace = document.BirthPlace ?? string.Empty,
                IssueDate = document.IssueDate,
                EducationDocumentName = document.EducationDocumentName ?? string.Empty,
                EducationLevel = document.EducationLevel ?? string.Empty,
                CreatedAt = document.CreatedAt
            });
        }

        return result;
    }

    public async Task<DownloadedFileDto> DownloadAsync(Guid applicantUserId, Guid documentId)
    {
        var document = await _repository.GetByIdAsync(documentId);
        if (document == null || document.ApplicantUserId != applicantUserId)
            throw new Exception("Документ не найден");

        var file = await _repository.GetStoredFileByIdAsync(document.StoredFileId);
        if (file == null)
            throw new Exception("Файл документа не найден");

        var content = await _fileStorageService.DownloadAsync(file.StoragePath);

        return new DownloadedFileDto
        {
            Content = content,
            FileName = file.FileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType
        };
    }

    public async Task DeleteAsync(Guid applicantUserId, Guid documentId)
    {
        await EnsureDocumentsEditableAsync(applicantUserId);

        var document = await _repository.GetByIdAsync(documentId);
        if (document == null || document.ApplicantUserId != applicantUserId)
            throw new Exception("Документ не найден");

        var file = await _repository.GetStoredFileByIdAsync(document.StoredFileId);
        if (file == null)
            throw new Exception("Файл документа не найден");

        await _fileStorageService.DeleteAsync(file.StoragePath);
        await _repository.DeleteDocumentAsync(document);
        await _repository.DeleteStoredFileAsync(file);
    }

    public async Task UpdateAsync(Guid applicantUserId, Guid documentId, UpdateDocumentRequest request)
    {
        await EnsureDocumentsEditableAsync(applicantUserId);

        var document = await _repository.GetByIdAsync(documentId);
        if (document == null || document.ApplicantUserId != applicantUserId)
            throw new Exception("Документ не найден");

        document.Type = request.Type;
        document.SeriesNumber = request.SeriesNumber ?? string.Empty;
        document.IssuedBy = request.IssuedBy ?? string.Empty;
        document.BirthPlace = request.BirthPlace ?? string.Empty;
        document.IssueDate = request.IssueDate.HasValue
            ? NormalizeUtc(request.IssueDate.Value)
            : null;
        document.EducationDocumentName = request.EducationDocumentName ?? string.Empty;
        document.EducationLevel = request.EducationLevel ?? string.Empty;

        await _repository.UpdateDocumentAsync(document);
    }

    public async Task ReplaceFileAsync(Guid applicantUserId, Guid documentId, ReplaceDocumentFileRequest request)
    {
        await EnsureDocumentsEditableAsync(applicantUserId);

        if (string.IsNullOrWhiteSpace(request.FileContentBase64))
            throw new Exception("Файл не передан");

        var document = await _repository.GetByIdAsync(documentId);
        if (document == null || document.ApplicantUserId != applicantUserId)
            throw new Exception("Документ не найден");

        var oldFile = await _repository.GetStoredFileByIdAsync(document.StoredFileId);
        if (oldFile == null)
            throw new Exception("Файл документа не найден");

        var fileBytes = Convert.FromBase64String(request.FileContentBase64);

        var newStoragePath = await _fileStorageService.UploadAsync(
            request.FileName,
            request.ContentType,
            fileBytes);

        var newFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            ContentType = request.ContentType,
            StoragePath = newStoragePath,
            UploadedAt = DateTime.UtcNow
        };

        await _repository.AddStoredFileAsync(newFile);

        document.StoredFileId = newFile.Id;
        await _repository.UpdateDocumentAsync(document);

        await _fileStorageService.DeleteAsync(oldFile.StoragePath);
        await _repository.DeleteStoredFileAsync(oldFile);
    }
}

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

    public DocumentServiceImpl(
        IDocumentRepository repository,
        IMessagePublisher messagePublisher,
        IFileStorageService fileStorageService)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _fileStorageService = fileStorageService;
    }

    public async Task UploadAsync(Guid applicantUserId, string applicantEmail, UploadDocumentRequest request)
    {
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

        var document = new Document
        {
            Id = Guid.NewGuid(),
            ApplicantUserId = applicantUserId,
            Type = request.Type,
            StoredFileId = file.Id,
            SeriesNumber = request.SeriesNumber,
            IssuedBy = request.IssuedBy,
            BirthPlace = request.BirthPlace,
            IssueDate = request.IssueDate,
            EducationDocumentName = request.EducationDocumentName,
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
                SeriesNumber = document.SeriesNumber,
                IssuedBy = document.IssuedBy,
                BirthPlace = document.BirthPlace,
                IssueDate = document.IssueDate,
                EducationDocumentName = document.EducationDocumentName,
                CreatedAt = document.CreatedAt
            });
        }

        return result;
    }
}

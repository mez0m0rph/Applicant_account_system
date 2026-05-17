using AdmissionService.Application.DTOs;
using AdmissionService.Application.Interfaces;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Shared.Contracts.Events;
using Shared.Messaging.Interfaces;

namespace AdmissionService.Infrastructure.Services;

public class AdmissionServiceImpl : IAdmissionService
{
    private readonly IAdmissionRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly int _maxPrograms;
    private readonly IProgramCatalogClient _programCatalogClient;
    private readonly IDocumentCatalogClient _documentCatalogClient;

    public AdmissionServiceImpl(
        IAdmissionRepository repository,
        IMessagePublisher messagePublisher,
        IConfiguration configuration,
        IProgramCatalogClient programCatalogClient,
        IDocumentCatalogClient documentCatalogClient)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _maxPrograms = configuration.GetValue<int>("AdmissionSettings:MaxPrograms", 5);
        _programCatalogClient = programCatalogClient;
        _documentCatalogClient = documentCatalogClient;
    }

    private static void EnsureAdmissionEditable(Admission admission)
    {
        if (admission.Status == AdmissionStatus.Closed)
            throw new Exception("Заявление закрыто и не может быть изменено");
    }

    private static void EnsurePriorityValid(int priority)
    {
        if (priority <= 0)
            throw new Exception("Приоритет должен быть больше 0");
    }

    private static void EnsurePriorityUnique(IEnumerable<AdmissionProgram> programs, Guid programId, int priority)
    {
        if (programs.Any(x => x.ProgramId != programId && x.Priority == priority))
            throw new Exception("Такой приоритет уже занят другой программой");
    }

    private static string NormalizeEducationLevel(string? level)
    {
        return (level ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static bool AreEducationLevelsCompatible(string existingLevel, string newLevel)
    {
        var left = NormalizeEducationLevel(existingLevel);
        var right = NormalizeEducationLevel(newLevel);

        if (left == right)
            return true;

        var pair = new HashSet<string> { left, right };

        if (pair.SetEquals(new[] { "бакалавриат", "специалитет" }))
            return true;

        return false;
    }

    private static bool IsProgramAllowedByEducationDocument(string documentLevel, string programLevel)
    {
        var doc = NormalizeEducationLevel(documentLevel);
        var program = NormalizeEducationLevel(programLevel);

        if (string.IsNullOrWhiteSpace(doc) || string.IsNullOrWhiteSpace(program))
            return true;

        if (doc == program)
            return true;

        return doc switch
        {
            "бакалавриат" => program is "бакалавриат" or "специалитет" or "магистратура",
            "специалитет" => program is "специалитет" or "магистратура" or "аспирантура",
            "магистратура" => program is "магистратура" or "аспирантура",
            "аспирантура" => program is "аспирантура",
            _ => true
        };
    }

    private async Task EnsureProgramAllowedByEducationDocumentAsync(Guid applicantUserId, string newProgramEducationLevel)
    {
        var documents = await _documentCatalogClient.GetByApplicantUserIdAsync(applicantUserId);

        var educationDocument = documents.FirstOrDefault(d =>
            string.Equals(d.Type, "EducationDocument", StringComparison.OrdinalIgnoreCase));

        if (educationDocument == null || string.IsNullOrWhiteSpace(educationDocument.EducationLevel))
            return;

        if (!IsProgramAllowedByEducationDocument(educationDocument.EducationLevel, newProgramEducationLevel))
            throw new Exception("Выбранная программа не соответствует уровню документа об образовании");
    }

    public async Task CreateAsync(Guid applicantUserId, string applicantEmail)
    {
        var existing = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (existing != null)
            throw new Exception("Заявление уже подано");

        var admission = new Admission
        {
            Id = Guid.NewGuid(),
            ApplicantUserId = applicantUserId,
            ApplicantEmail = applicantEmail,
            Status = AdmissionStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(admission);
    }

    public async Task<AdmissionResponse?> GetMyAsync(Guid applicantUserId)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            return null;

        return await MapAsync(admission);
    }

    public async Task<List<AdmissionResponse>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        var result = new List<AdmissionResponse>();

        foreach (var item in items)
            result.Add(await MapAsync(item));

        return result;
    }

    public async Task<PagedAdmissionsResponse> GetPagedAsync(GetAdmissionsQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(query);

        var mapped = new List<AdmissionResponse>();
        foreach (var item in items)
            mapped.Add(await MapAsync(item));

        return new PagedAdmissionsResponse
        {
            Items = mapped,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task AddProgramAsync(Guid applicantUserId, Guid programId, int priority)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            throw new Exception("Сначала подайте заявление");

        EnsureAdmissionEditable(admission);
        EnsurePriorityValid(priority);

        var currentPrograms = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);
        if (currentPrograms.Count >= _maxPrograms)
            throw new Exception($"Нельзя выбрать больше {_maxPrograms} программ");

        if (currentPrograms.Any(x => x.ProgramId == programId))
            throw new Exception("Программа уже добавлена в заявление");

        var newProgram = await _programCatalogClient.GetByIdAsync(programId);
        if (newProgram == null)
            throw new Exception("Программа не найдена");

        await EnsureProgramAllowedByEducationDocumentAsync(applicantUserId, newProgram.EducationLevel);

        foreach (var selectedProgram in currentPrograms)
        {
            var existingProgram = await _programCatalogClient.GetByIdAsync(selectedProgram.ProgramId);
            if (existingProgram == null)
                continue;

            if (!AreEducationLevelsCompatible(existingProgram.EducationLevel, newProgram.EducationLevel))
                throw new Exception("Нельзя выбирать программы с несовместимыми уровнями образования");
        }

        EnsurePriorityUnique(currentPrograms, programId, priority);

        var item = new AdmissionProgram
        {
            Id = Guid.NewGuid(),
            AdmissionId = admission.Id,
            ProgramId = programId,
            Priority = priority
        };

        await _repository.AddProgramAsync(item);

        admission.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task UpdateProgramPriorityAsync(Guid applicantUserId, Guid programId, int priority)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureAdmissionEditable(admission);
        EnsurePriorityValid(priority);

        var existing = await _repository.GetProgramAsync(admission.Id, programId);
        if (existing == null)
            throw new Exception("Программа не найдена в заявлении");

        var currentPrograms = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);
        EnsurePriorityUnique(currentPrograms, programId, priority);

        existing.Priority = priority;
        await _repository.UpdateProgramAsync(existing);

        admission.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task RemoveProgramAsync(Guid applicantUserId, Guid programId)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureAdmissionEditable(admission);

        var existing = await _repository.GetProgramAsync(admission.Id, programId);
        if (existing == null)
            throw new Exception("Программа не найдена в заявлении");

        await _repository.RemoveProgramAsync(existing);

        admission.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        admission.AssignedManagerUserId = managerUserId;
        admission.Status = AdmissionStatus.OnReview;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = admission.ApplicantUserId,
            Email = admission.ApplicantEmail,
            Subject = "Назначен менеджер",
            Message = $"На ваше заявление назначен менеджер: {managerEmail}"
        });

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = managerUserId,
            Email = managerEmail,
            Subject = "Вам назначено заявление",
            Message = $"Вам назначено заявление абитуриента {admission.ApplicantEmail}"
        });
    }

    public async Task ReleaseManagerAsync(Guid admissionId)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        admission.AssignedManagerUserId = null;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task UpdateStatusAsync(Guid admissionId, string status)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        if (!Enum.TryParse<AdmissionStatus>(status, true, out var parsedStatus))
            throw new Exception("Некорректный статус");

        admission.Status = parsedStatus;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = admission.ApplicantUserId,
            Email = admission.ApplicantEmail,
            Subject = "Изменение статуса заявления",
            Message = $"Статус вашего заявления изменен на {parsedStatus}"
        });
    }

    private async Task<AdmissionResponse> MapAsync(Admission admission)
    {
        var programs = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);

        return new AdmissionResponse
        {
            Id = admission.Id,
            ApplicantUserId = admission.ApplicantUserId,
            ApplicantEmail = admission.ApplicantEmail,
            Status = admission.Status.ToString(),
            AssignedManagerUserId = admission.AssignedManagerUserId,
            CreatedAt = admission.CreatedAt,
            UpdatedAt = admission.UpdatedAt,
            Programs = programs.Select(x => new AdmissionProgramItemResponse
            {
                ProgramId = x.ProgramId,
                Priority = x.Priority
            }).ToList()
        };
    }
}

using AdmissionService.Application.DTOs;
using AdmissionService.Application.DTOs.External;
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
    private readonly IProgramCatalogClient _programCatalogClient;
    private readonly IApplicantCatalogClient _applicantCatalogClient;
    private readonly IManagerCatalogClient _managerCatalogClient;
    private readonly IConfiguration _configuration;

    public AdmissionServiceImpl(
        IAdmissionRepository repository,
        IMessagePublisher messagePublisher,
        IProgramCatalogClient programCatalogClient,
        IApplicantCatalogClient applicantCatalogClient,
        IManagerCatalogClient managerCatalogClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _programCatalogClient = programCatalogClient;
        _applicantCatalogClient = applicantCatalogClient;
        _managerCatalogClient = managerCatalogClient;
        _configuration = configuration;
    }

    public async Task CreateAsync(Guid applicantUserId, string applicantEmail)
    {
        var existing = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (existing != null)
            throw new Exception("Заявление уже существует");

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
        return admission == null ? null : await MapAsync(admission);
    }

    public async Task<List<AdmissionResponse>> GetAllAsync()
    {
        var admissions = await _repository.GetAllAsync();
        var result = new List<AdmissionResponse>();

        foreach (var admission in admissions)
            result.Add(await MapAsync(admission));

        return result;
    }

    public async Task<PagedAdmissionsResponse> GetPagedAsync(GetAdmissionsQuery query, Guid? currentUserId, string? currentRole)
    {
        if (string.Equals(currentRole, "Manager", StringComparison.OrdinalIgnoreCase) && query.OnlyMine && currentUserId.HasValue)
            query.AssignedManagerUserId = currentUserId;

        var result = await _repository.GetPagedAsync(query);
        var mapped = new List<AdmissionResponse>();

        foreach (var item in result.Items)
            mapped.Add(await MapAsync(item));

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            mapped = mapped.Where(x =>
                    x.ApplicantEmail.ToLowerInvariant().Contains(search) ||
                    x.ApplicantFullName.ToLowerInvariant().Contains(search))
                .ToList();
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        return new PagedAdmissionsResponse
        {
            Items = mapped,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalCount == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)pageSize)
        };
    }

    public async Task AddProgramAsync(Guid applicantUserId, Guid programId, int priority)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureAdmissionEditable(admission);
        EnsurePriorityValid(priority);

        var currentPrograms = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);
        var maxPrograms = _configuration.GetValue<int?>("AdmissionSettings:MaxPrograms") ?? 5;

        if (currentPrograms.Count >= maxPrograms)
            throw new Exception($"Нельзя выбрать больше {maxPrograms} программ");

        if (currentPrograms.Any(x => x.ProgramId == programId))
            throw new Exception("Программа уже добавлена");

        EnsurePriorityUnique(currentPrograms, null, priority);

        var selectedLevels = new List<string>();
        foreach (var item in currentPrograms)
        {
            var dto = await _programCatalogClient.GetByIdAsync(item.ProgramId);
            if (dto != null && !string.IsNullOrWhiteSpace(dto.EducationLevel))
                selectedLevels.Add(dto.EducationLevel);
        }

        var program = await _programCatalogClient.GetByIdAsync(programId);
        if (program == null)
            throw new Exception("Программа не найдена");

        EnsureEducationLevelRules(selectedLevels, program.EducationLevel ?? string.Empty);

        await _repository.AddProgramAsync(new AdmissionProgram
        {
            Id = Guid.NewGuid(),
            AdmissionId = admission.Id,
            ProgramId = programId,
            Priority = priority
        });

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

    public async Task AddProgramForStaffAsync(Guid admissionId, Guid programId, int priority, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureCanManageAdmission(admission, currentUserId, currentRole);
        await AddProgramAsync(admission.ApplicantUserId, programId, priority);
    }

    public async Task UpdateProgramPriorityForStaffAsync(Guid admissionId, Guid programId, int priority, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureCanManageAdmission(admission, currentUserId, currentRole);
        await UpdateProgramPriorityAsync(admission.ApplicantUserId, programId, priority);
    }

    public async Task RemoveProgramForStaffAsync(Guid admissionId, Guid programId, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureCanManageAdmission(admission, currentUserId, currentRole);
        await RemoveProgramAsync(admission.ApplicantUserId, programId);
    }

    public async Task TakeAdmissionAsync(Guid admissionId, Guid managerUserId)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        if (admission.AssignedManagerUserId.HasValue && admission.AssignedManagerUserId.Value != managerUserId)
            throw new Exception("Поступление уже назначено другому менеджеру");

        var manager = await _managerCatalogClient.GetByUserIdAsync(managerUserId);

        admission.AssignedManagerUserId = managerUserId;
        admission.Status = AdmissionStatus.OnReview;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = admission.ApplicantUserId,
            Email = admission.ApplicantEmail,
            Subject = "Поступление взято в работу",
            Message = "Ваше заявление взято в работу менеджером."
        });

        if (manager != null && !string.IsNullOrWhiteSpace(manager.Email))
        {
            await _messagePublisher.PublishAsync(new NotificationRequestedEvent
            {
                UserId = managerUserId,
                Email = manager.Email,
                Subject = "Вам назначено заявление",
                Message = $"Вы взяли в работу заявление абитуриента {admission.ApplicantEmail}"
            });
        }
    }

    public async Task ReleaseOwnAdmissionAsync(Guid admissionId, Guid managerUserId)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        if (admission.AssignedManagerUserId != managerUserId)
            throw new Exception("Можно отказаться только от своего поступления");

        admission.AssignedManagerUserId = null;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsurePrivilegedStaff(currentRole);

        if (admission.AssignedManagerUserId.HasValue)
            throw new Exception("Менеджера можно назначить только на свободное поступление");

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

    public async Task ReleaseManagerAsync(Guid admissionId, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsurePrivilegedStaff(currentRole);

        admission.AssignedManagerUserId = null;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);
    }

    public async Task UpdateStatusAsync(Guid admissionId, string status, Guid? currentUserId, string? currentRole)
    {
        var admission = await _repository.GetByIdAsync(admissionId);
        if (admission == null)
            throw new Exception("Заявление не найдено");

        EnsureCanManageAdmission(admission, currentUserId, currentRole);

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

    private static void EnsureAdmissionEditable(Admission admission)
    {
        if (admission.Status == AdmissionStatus.Closed)
            throw new Exception("Заявление закрыто для редактирования");
    }

    private static void EnsurePriorityValid(int priority)
    {
        if (priority < 1)
            throw new Exception("Приоритет должен быть больше 0");
    }

    private static void EnsurePriorityUnique(List<AdmissionProgram> currentPrograms, Guid? currentProgramId, int priority)
    {
        if (currentPrograms.Any(x => x.Priority == priority && x.ProgramId != currentProgramId))
            throw new Exception("Программа с таким приоритетом уже существует");
    }

    private static void EnsureEducationLevelRules(List<string> selectedLevels, string newLevel)
    {
        if (selectedLevels.Count == 0)
            return;

        var distinctLevels = selectedLevels
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctLevels.Count > 1)
            throw new Exception("Уже выбраны программы разных ступеней обучения");

        var selectedLevel = distinctLevels.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(selectedLevel) &&
            !string.Equals(selectedLevel, newLevel, StringComparison.OrdinalIgnoreCase))
            throw new Exception("Нельзя выбрать программы разных ступеней обучения");
    }

    private static bool IsPrivilegedRole(string? currentRole)
    {
        return string.Equals(currentRole, "MainManager", StringComparison.OrdinalIgnoreCase)
            || string.Equals(currentRole, "Admin", StringComparison.OrdinalIgnoreCase);
    }

    private static void EnsurePrivilegedStaff(string? currentRole)
    {
        if (!IsPrivilegedRole(currentRole))
            throw new Exception("Недостаточно прав");
    }

    private static void EnsureCanManageAdmission(Admission admission, Guid? currentUserId, string? currentRole)
    {
        if (IsPrivilegedRole(currentRole))
            return;

        if (string.Equals(currentRole, "Manager", StringComparison.OrdinalIgnoreCase)
            && currentUserId.HasValue
            && admission.AssignedManagerUserId == currentUserId.Value)
            return;

        throw new Exception("Недостаточно прав для изменения этого заявления");
    }

    private async Task<AdmissionResponse> MapAsync(Admission admission)
    {
        var programs = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);
        var applicant = await _applicantCatalogClient.GetByUserIdAsync(admission.ApplicantUserId);
        var manager = admission.AssignedManagerUserId.HasValue
            ? await _managerCatalogClient.GetByUserIdAsync(admission.AssignedManagerUserId.Value)
            : null;

        var mappedPrograms = new List<AdmissionProgramItemResponse>();
        foreach (var x in programs)
        {
            var program = await _programCatalogClient.GetByIdAsync(x.ProgramId);

            mappedPrograms.Add(new AdmissionProgramItemResponse
            {
                ProgramId = x.ProgramId,
                Priority = x.Priority,
                ProgramCode = program?.Code ?? string.Empty,
                ProgramTitle = program?.Title ?? string.Empty
            });
        }

        return new AdmissionResponse
        {
            Id = admission.Id,
            ApplicantUserId = admission.ApplicantUserId,
            ApplicantEmail = admission.ApplicantEmail,
            ApplicantFullName = applicant?.FullName ?? string.Empty,
            Status = admission.Status.ToString(),
            AssignedManagerUserId = admission.AssignedManagerUserId,
            AssignedManagerName = manager?.FullName ?? string.Empty,
            AssignedManagerEmail = manager?.Email ?? string.Empty,
            CreatedAt = admission.CreatedAt,
            UpdatedAt = admission.UpdatedAt,
            Programs = mappedPrograms
        };
    }
}

using AdmissionService.Application.DTOs;
using AdmissionService.Application.Interfaces;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.Enums;
using Shared.Contracts.Events;
using Shared.Messaging.Interfaces;

namespace AdmissionService.Infrastructure.Services;

public class AdmissionServiceImpl : IAdmissionService
{
    private readonly IAdmissionRepository _repository;
    private readonly IMessagePublisher _messagePublisher;

    private static void EnsureAdmissionEditable(Admission admission)
    {
        if (admission.Status == AdmissionStatus.Closed) 
            throw new Exception("Заявление закрыто и не может быть изменено");
    }

    public AdmissionServiceImpl(IAdmissionRepository repository, IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
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

    public async Task AddProgramAsync(Guid applicantUserId, Guid programId, int priority)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);
        if (admission == null)
            throw new Exception("Сначала подайте заявление");

        EnsureAdmissionEditable(admission);

        var existing = await _repository.GetProgramAsync(admission.Id, programId);
        if (existing != null)
            throw new Exception("Программа уже добавлена в заявление");

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

        var existing = await _repository.GetProgramAsync(admission.Id, programId);
        if (existing == null)
            throw new Exception("Программа не найдена в заявлении");

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

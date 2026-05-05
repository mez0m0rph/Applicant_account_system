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

    public AdmissionServiceImpl(
        IAdmissionRepository repository,
        IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
    }

    public async Task CreateAdmissionAsync(Guid applicantUserId, string applicantEmail, CreateAdmissionRequest request)
    {
        var existingAdmission = await _repository.GetByApplicantUserIdAsync(applicantUserId);

        if (existingAdmission != null)
            throw new Exception("Заявление уже подано");

        var admission = new Admission
        {
            Id = Guid.NewGuid(),
            ApplicantUserId = applicantUserId,
            ApplicantEmail = applicantEmail,
            Status = AdmissionStatus.Created,
            AssignedManagerUserId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAdmissionAsync(admission);

        var admissionPrograms = request.Programs
            .Select(p => new AdmissionProgram
            {
                Id = Guid.NewGuid(),
                AdmissionId = admission.Id,
                ProgramId = p.ProgramId,
                Priority = p.Priority
            })
            .ToList();

        await _repository.CreateAdmissionProgramsAsync(admissionPrograms);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = applicantUserId,
            Email = applicantEmail,
            Subject = "Заявление создано",
            Message = "Ваше заявление успешно создано."
        });
    }

    public async Task<AdmissionResponse> GetMyAdmissionAsync(Guid applicantUserId)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);

        if (admission == null)
            throw new Exception("Заявление не было создано");

        var programs = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);

        return Map(admission, programs);
    }

    public async Task<List<AdmissionResponse>> GetAllAsync()
    {
        var admissions = await _repository.GetAllAsync();
        var result = new List<AdmissionResponse>();

        foreach (var admission in admissions)
        {
            var programs = await _repository.GetProgramsByAdmissionIdAsync(admission.Id);
            result.Add(Map(admission, programs));
        }

        return result;
    }

    public async Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail)
    {
        var admission = await _repository.GetByIdAsync(admissionId);

        if (admission == null)
            throw new Exception("Заявление не найдено");

        if (admission.AssignedManagerUserId != null)
            throw new Exception("Менеджер уже назначен");

        admission.AssignedManagerUserId = managerUserId;
        admission.Status = AdmissionStatus.OnReview;
        admission.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAdmissionAsync(admission);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = managerUserId,
            Email = managerEmail,
            Subject = "Вам назначено поступление",
            Message = $"Вам назначено поступление {admission.Id}."
        });

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = admission.ApplicantUserId,
            Email = admission.ApplicantEmail,
            Subject = "Назначен менеджер",
            Message = $"Для вашего поступления назначен менеджер. Статус заявления: {admission.Status}."
        });
    }

    public async Task ReleaseManagerAsync(Guid admissionId)
    {
        var admission = await _repository.GetByIdAsync(admissionId);

        if (admission == null)
            throw new Exception("Заявление не найдено");

        admission.AssignedManagerUserId = null;
        admission.Status = AdmissionStatus.Created;
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
            Subject = "Статус поступления изменен",
            Message = $"Новый статус вашего заявления: {admission.Status}."
        });
    }

    private static AdmissionResponse Map(Admission admission, List<AdmissionProgram> programs)
    {
        return new AdmissionResponse
        {
            Id = admission.Id,
            ApplicantUserId = admission.ApplicantUserId,
            ApplicantEmail = admission.ApplicantEmail,
            Status = admission.Status.ToString(),
            AssignedManagerUserId = admission.AssignedManagerUserId,
            CreatedAt = admission.CreatedAt,
            UpdatedAt = admission.UpdatedAt,
            Programs = programs
                .Select(p => new AdmissionProgramDto
                {
                    ProgramId = p.ProgramId,
                    Priority = p.Priority
                })
                .ToList()
        };
    }
}

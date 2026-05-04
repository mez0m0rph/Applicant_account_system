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

        return new AdmissionResponse
        {
            Id = admission.Id,
            ApplicantUserId = admission.ApplicantUserId,
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

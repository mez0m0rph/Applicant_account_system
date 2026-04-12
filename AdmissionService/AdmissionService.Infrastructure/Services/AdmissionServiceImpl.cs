using AdmissionService.Application.DTOs;
using AdmissionService.Application.Interfaces;
using AdmissionService.Domain.Entities;
using AdmissionService.Domain.Enums;

namespace AdmissionService.Infrastructure.Services;

public class AdmissionServiceImpl : IAdmissionService
{
    private readonly IAdmissionRepository _repository;
    public AdmissionServiceImpl(IAdmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAdmissionAsync(Guid applicantUserId, CreateAdmissionRequest request)
    {
        var existingAdmission = await _repository.GetByApplicantUserIdAsync(applicantUserId);

        if (existingAdmission != null)
            throw new Exception("заявление уже подано");

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
    }

    public async Task<AdmissionResponse> GetMyAdmissionAsync(Guid applicantUserId)
    {
        var admission = await _repository.GetByApplicantUserIdAsync(applicantUserId);

        if (admission == null)
            throw new Exception("заявление не было создано");

        return new AdmissionResponse
        {
            Id = admission.Id,
            ApplicantUserId = admission.ApplicantUserId,
            Status = admission.Status.ToString(),
            AssignedManagerUserId = admission.AssignedManagerUserId,
            CreatedAt = admission.CreatedAt,
            UpdatedAt = admission.UpdatedAt,
            Programs = new List<AdmissionProgramDto>()
        };
    }
}
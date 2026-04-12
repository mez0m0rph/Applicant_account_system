using AdmissionService.Application.DTOs;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionService
{
    Task CreateAdmissionAsync(Guid applicantUserId, CreateAdmissionRequest request);
    Task<AdmissionResponse> GetMyAdmissionAsync(Guid applicantUserId);
}
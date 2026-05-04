using AdmissionService.Application.DTOs;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionService
{
    Task CreateAdmissionAsync(Guid applicantUserId, string applicantEmail, CreateAdmissionRequest request);
    Task<AdmissionResponse> GetMyAdmissionAsync(Guid applicantUserId);
}

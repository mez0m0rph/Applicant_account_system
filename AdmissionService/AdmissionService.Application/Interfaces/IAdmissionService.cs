using AdmissionService.Application.DTOs;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionService
{
    Task CreateAdmissionAsync(Guid applicantUserId, string applicantEmail, CreateAdmissionRequest request);
    Task<AdmissionResponse> GetMyAdmissionAsync(Guid applicantUserId);
    Task<List<AdmissionResponse>> GetAllAsync();
    Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail);
    Task ReleaseManagerAsync(Guid admissionId);
    Task UpdateStatusAsync(Guid admissionId, string status);
}

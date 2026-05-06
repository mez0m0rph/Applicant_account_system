using AdmissionService.Application.DTOs;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionService
{
    Task CreateAsync(Guid applicantUserId, string applicantEmail);
    Task<AdmissionResponse?> GetMyAsync(Guid applicantUserId);
    Task<List<AdmissionResponse>> GetAllAsync();

    Task AddProgramAsync(Guid applicantUserId, Guid programId, int priority);
    Task UpdateProgramPriorityAsync(Guid applicantUserId, Guid programId, int priority);
    Task RemoveProgramAsync(Guid applicantUserId, Guid programId);

    Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail);
    Task ReleaseManagerAsync(Guid admissionId);
    Task UpdateStatusAsync(Guid admissionId, string status);
}

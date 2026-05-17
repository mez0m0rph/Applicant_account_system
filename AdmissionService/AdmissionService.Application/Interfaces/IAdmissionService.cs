using AdmissionService.Application.DTOs;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionService
{
    Task CreateAsync(Guid applicantUserId, string applicantEmail);
    Task<AdmissionResponse?> GetMyAsync(Guid applicantUserId);
    Task<List<AdmissionResponse>> GetAllAsync();
    Task<PagedAdmissionsResponse> GetPagedAsync(GetAdmissionsQuery query, Guid? currentUserId, string? currentRole);

    Task AddProgramAsync(Guid applicantUserId, Guid programId, int priority);
    Task UpdateProgramPriorityAsync(Guid applicantUserId, Guid programId, int priority);
    Task RemoveProgramAsync(Guid applicantUserId, Guid programId);

    Task AssignManagerAsync(Guid admissionId, Guid managerUserId, string managerEmail, Guid? currentUserId, string? currentRole);
    Task ReleaseManagerAsync(Guid admissionId, Guid? currentUserId, string? currentRole);
    Task UpdateStatusAsync(Guid admissionId, string status, Guid? currentUserId, string? currentRole);
}

using AdmissionService.Application.DTOs;
using AdmissionService.Domain.Entities;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionRepository
{
    Task<Admission?> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task<Admission?> GetByIdAsync(Guid id);
    Task<List<Admission>> GetAllAsync();
    Task<(List<Admission> Items, int TotalCount)> GetPagedAsync(GetAdmissionsQuery query);
    Task CreateAsync(Admission admission);
    Task UpdateAdmissionAsync(Admission admission);

    Task<List<AdmissionProgram>> GetProgramsByAdmissionIdAsync(Guid admissionId);
    Task<AdmissionProgram?> GetProgramAsync(Guid admissionId, Guid programId);
    Task AddProgramAsync(AdmissionProgram program);
    Task UpdateProgramAsync(AdmissionProgram program);
    Task RemoveProgramAsync(AdmissionProgram program);
}

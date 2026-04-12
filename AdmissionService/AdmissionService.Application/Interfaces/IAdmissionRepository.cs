using AdmissionService.Domain.Entities;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionRepository
{
    Task<Admission?> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task CreateAdmissionAsync(Admission admission);
    Task CreateAdmissionProgramsAsync(List<AdmissionProgram> programs);
    Task<List<AdmissionProgram>> GetProgramsByAdmissionIdAsync(Guid admissionId);
}
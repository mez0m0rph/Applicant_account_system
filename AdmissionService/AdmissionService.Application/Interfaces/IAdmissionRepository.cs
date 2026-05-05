using AdmissionService.Domain.Entities;

namespace AdmissionService.Application.Interfaces;

public interface IAdmissionRepository
{
    Task<List<Admission>> GetAllAsync();
    Task<Admission?> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task<Admission?> GetByIdAsync(Guid admissionId);

    Task CreateAdmissionAsync(Admission admission);
    Task UpdateAdmissionAsync(Admission admission);

    Task CreateAdmissionProgramsAsync(List<AdmissionProgram> programs);
    Task<List<AdmissionProgram>> GetProgramsByAdmissionIdAsync(Guid admissionId);
}

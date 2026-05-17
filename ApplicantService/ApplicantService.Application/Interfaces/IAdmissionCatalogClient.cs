using ApplicantService.Application.DTOs.External;

namespace ApplicantService.Application.Interfaces;

public interface IAdmissionCatalogClient
{
    Task<AdmissionAccessDto?> GetByApplicantUserIdAsync(Guid applicantUserId);
}

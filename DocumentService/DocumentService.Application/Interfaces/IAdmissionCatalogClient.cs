using DocumentService.Application.DTOs.External;

namespace DocumentService.Application.Interfaces;

public interface IAdmissionCatalogClient
{
    Task<AdmissionDetailsDto?> GetByApplicantUserIdAsync(Guid applicantUserId);
}
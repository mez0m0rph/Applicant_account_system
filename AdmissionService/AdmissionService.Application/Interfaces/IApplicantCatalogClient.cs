using AdmissionService.Application.DTOs.External;

namespace AdmissionService.Application.Interfaces;

public interface IApplicantCatalogClient
{
    Task<ApplicantProfileDto?> GetByUserIdAsync(Guid userId);
}

using ApplicantService.Application.DTOs.External;

namespace ApplicantService.Application.Interfaces;

public interface IAdmissionCatalogClient
{
    Task<AdmissionDetailsDto?> GetMyAsync(Guid applicantUserId);
}
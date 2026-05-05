using ApplicantService.Application.DTOs;

namespace ApplicantService.Application.Interfaces;

public interface IApplicantService
{
    Task CreateAsync(Guid userId, CreateApplicantRequest request);
    Task<ApplicantResponse?> GetMyAsync(Guid userId);
    Task UpdateAsync(Guid userId, UpdateApplicantRequest request);
}

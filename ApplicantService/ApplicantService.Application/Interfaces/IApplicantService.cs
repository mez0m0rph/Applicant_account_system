using ApplicantService.Application.DTOs;

namespace ApplicantService.Application.Interfaces;

public interface IApplicantService
{
    Task CreateAsync(Guid userId, CreateRequest request);
    Task<GetProfileResponse> GetMyProfileAsync(Guid userId);
    Task UpdateAsync(Guid userId, UpdateProfileRequest request);
}
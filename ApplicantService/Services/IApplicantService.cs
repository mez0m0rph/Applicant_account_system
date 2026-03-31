using ApplicantService.DTOs;

namespace ApplicantService.Services;

public interface IApplicantService
{
    Task CreateAsync(CreateRequest request);
    Task LoginAsync(LoginRequest request);
}
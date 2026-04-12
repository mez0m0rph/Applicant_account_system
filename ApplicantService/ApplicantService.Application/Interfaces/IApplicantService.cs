using ApplicantService.Application.DTOs;

namespace ApplicantService.Application.Interfaces;

public interface IApplicantService
{
    Task CreateAsync(CreateRequest request);
    Task LoginAsync(LoginRequest request);
}
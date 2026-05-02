using ApplicantService.Domain.Entities;

namespace ApplicantService.Application.Interfaces;

public interface IApplicantRepository
{
    Task<Applicant?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(Applicant applicant);
    Task UpdateAsync(Applicant applicant);
}
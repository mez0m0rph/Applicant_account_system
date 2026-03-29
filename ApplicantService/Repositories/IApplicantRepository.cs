using ApplicantService.Models;

namespace ApplicantService.Repositories;

public interface IApplicantRepository
{
    Task<Applicant?> GetByUserIdAsync(Guid userId);

    Task CreateAsync(Applicant applicant);
    
    Task UpdateAsync(Applicant applicant);
}
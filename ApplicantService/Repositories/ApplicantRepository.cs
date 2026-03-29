using ApplicantService.Data;
using ApplicantService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicantService.Repositories;

public class ApplicantRepository : IApplicantRepository
{
    private readonly AppDbContext _context;
    public ApplicantRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Applicant?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task CreateAsync(Applicant applicant)
    {
        await _context.Applicants.AddAsync(applicant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Applicant applicant)
    {
        _context.Applicants.Update(applicant);
        await _context.SaveChangesAsync();
    }
}
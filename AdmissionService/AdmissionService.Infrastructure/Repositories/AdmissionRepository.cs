using AdmissionService.Application.Interfaces;
using AdmissionService.Domain.Entities;
using AdmissionService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Repositories;

public class AdmissionRepository : IAdmissionRepository
{
    private readonly AppDbContext _context;
    public AdmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Admission?> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        return await _context.Admissions.FirstOrDefaultAsync(a => a.ApplicantUserId == applicantUserId);
    }

    public async Task CreateAdmissionAsync(Admission admission)
    {
        await _context.Admissions.AddAsync(admission);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAdmissionProgramsAsync(List<AdmissionProgram> programs)
    {
        await _context.AdmissionPrograms.AddRangeAsync(programs);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AdmissionProgram>> GetProgramsByAdmissionIdAsync(Guid admissionId)
    {
        return await _context.AdmissionPrograms
            .Where(p => p.AdmissionId == admissionId)
            .OrderBy(p => p.Priority)
            .ToListAsync();
    }
}
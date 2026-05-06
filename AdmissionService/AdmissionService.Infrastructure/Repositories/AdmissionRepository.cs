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
        return await _context.Admissions.FirstOrDefaultAsync(x => x.ApplicantUserId == applicantUserId);
    }

    public async Task<Admission?> GetByIdAsync(Guid id)
    {
        return await _context.Admissions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Admission>> GetAllAsync()
    {
        return await _context.Admissions
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Admission admission)
    {
        await _context.Admissions.AddAsync(admission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAdmissionAsync(Admission admission)
    {
        _context.Admissions.Update(admission);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AdmissionProgram>> GetProgramsByAdmissionIdAsync(Guid admissionId)
    {
        return await _context.AdmissionPrograms
            .Where(x => x.AdmissionId == admissionId)
            .OrderBy(x => x.Priority)
            .ToListAsync();
    }

    public async Task<AdmissionProgram?> GetProgramAsync(Guid admissionId, Guid programId)
    {
        return await _context.AdmissionPrograms
            .FirstOrDefaultAsync(x => x.AdmissionId == admissionId && x.ProgramId == programId);
    }

    public async Task AddProgramAsync(AdmissionProgram program)
    {
        await _context.AdmissionPrograms.AddAsync(program);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProgramAsync(AdmissionProgram program)
    {
        _context.AdmissionPrograms.Update(program);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveProgramAsync(AdmissionProgram program)
    {
        _context.AdmissionPrograms.Remove(program);
        await _context.SaveChangesAsync();
    }
}

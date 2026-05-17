using AdmissionService.Application.DTOs;
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

    public async Task<(List<Admission> Items, int TotalCount)> GetPagedAsync(GetAdmissionsQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var admissions = _context.Admissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLower();
            admissions = admissions.Where(x => x.Status.ToString().ToLower() == status);
        }

        if (query.OnlyUnassigned)
        {
            admissions = admissions.Where(x => x.AssignedManagerUserId == null);
        }

        if (query.AssignedManagerUserId.HasValue)
        {
            admissions = admissions.Where(x => x.AssignedManagerUserId == query.AssignedManagerUserId.Value);
        }

        if (query.ProgramId.HasValue)
        {
            var admissionIds = _context.AdmissionPrograms
                .Where(x => x.ProgramId == query.ProgramId.Value)
                .Select(x => x.AdmissionId);

            admissions = admissions.Where(x => admissionIds.Contains(x.Id));
        }

        admissions = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
        {
            ("updatedat", "asc") => admissions.OrderBy(x => x.UpdatedAt),
            ("createdat", "asc") => admissions.OrderBy(x => x.CreatedAt),
            ("createdat", _) => admissions.OrderByDescending(x => x.CreatedAt),
            (_, "asc") => admissions.OrderBy(x => x.UpdatedAt),
            _ => admissions.OrderByDescending(x => x.UpdatedAt)
        };

        var totalCount = await admissions.CountAsync();

        var items = await admissions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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

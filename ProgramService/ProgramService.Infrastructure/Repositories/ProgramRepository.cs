using Microsoft.EntityFrameworkCore;
using ProgramService.Application.Interfaces;
using ProgramService.Domain.Entities;
using ProgramService.Infrastructure.Data;

namespace ProgramService.Infrastructure.Repositories;

public class ProgramRepository : IProgramRepository
{
    private readonly AppDbContext _context;

    public ProgramRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudyProgram>> GetAllAsync()
    {
        return await _context.StudyPrograms
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public async Task<StudyProgram?> GetByIdAsync(Guid id)
    {
        return await _context.StudyPrograms
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<StudyProgram?> GetByExternalIdAsync(string externalId)
    {
        return await _context.StudyPrograms
            .FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task CreateAsync(StudyProgram program)
    {
        await _context.StudyPrograms.AddAsync(program);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudyProgram program)
    {
        _context.StudyPrograms.Update(program);
        await _context.SaveChangesAsync();
    }
}

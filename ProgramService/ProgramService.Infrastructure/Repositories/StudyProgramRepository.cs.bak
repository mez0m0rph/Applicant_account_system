using Microsoft.EntityFrameworkCore;
using ProgramService.Infrastructure.Data;
using ProgramService.Application.Interfaces;
using ProgramService.Domain.Entities;

namespace ProgramService.Infrastructure.Repositories;

public class StudyProgramRepository : IStudyProgramRepository
{
    private readonly AppDbContext _context;

    public StudyProgramRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudyProgram>> GetAllAsync()
    {
        return await _context.StudyPrograms.ToListAsync();
    }

    public async Task<StudyProgram?> GetByIdAsync(Guid id)
    {
        return await _context.StudyPrograms.FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<StudyProgram?> GetByExternalIdAsync(string externalId)
    {
        return await _context.StudyPrograms.FirstOrDefaultAsync(pr => pr.ExternalId == externalId);
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

    public async Task DeleteAsync(Guid id)
    {
        var program = await _context.StudyPrograms.FirstOrDefaultAsync(pr => pr.Id == id);

        if (program != null)
        {
            _context.StudyPrograms.Remove(program);
            await _context.SaveChangesAsync();
        }
    }
}
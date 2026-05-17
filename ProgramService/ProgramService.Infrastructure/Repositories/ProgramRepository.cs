using Microsoft.EntityFrameworkCore;
using ProgramService.Application.DTOs;
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

    public async Task<(List<StudyProgram> Items, int TotalCount)> GetPagedAsync(GetProgramsQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var programs = _context.StudyPrograms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            programs = programs.Where(x =>
                x.Code.ToLower().Contains(search) ||
                x.Title.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Faculty))
        {
            var faculty = query.Faculty.Trim().ToLower();
            programs = programs.Where(x => x.Faculty.ToLower().Contains(faculty));
        }

        if (!string.IsNullOrWhiteSpace(query.EducationLevel))
        {
            var level = query.EducationLevel.Trim().ToLower();
            programs = programs.Where(x => x.EducationLevel.ToLower().Contains(level));
        }

        if (!string.IsNullOrWhiteSpace(query.EducationForm))
        {
            var form = query.EducationForm.Trim().ToLower();
            programs = programs.Where(x => x.EducationForm.ToLower().Contains(form));
        }

        if (!string.IsNullOrWhiteSpace(query.Language))
        {
            var language = query.Language.Trim().ToLower();
            programs = programs.Where(x => x.Language.ToLower().Contains(language));
        }

        var totalCount = await programs.CountAsync();

        var items = await programs
            .OrderBy(x => x.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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

using ProgramService.DTOs;
using ProgramService.Models;
using ProgramService.Repositories;

namespace ProgramService.Services;

public class StudyProgramService : IStudyProgramService
{
    private readonly IStudyProgramRepository _repository;

    public StudyProgramService(IStudyProgramRepository repository)
    {
        _repository = repository;
    }

    private ProgramDto MaptoDto(StudyProgram program)
    {
        return new ProgramDto
        {
            Id = program.Id,
            ExternalId = program.ExternalId,
            Title = program.Title,
            Description = program.Description,
            BudgetPlaces = program.BudgetPlaces,
            PaidPlaces = program.PaidPlaces,
            Faculty = program.Faculty,
            Duration = program.Duration,
            Degree = program.Degree
        };
    }

    public async Task<List<ProgramDto>> GetAllAsync()
    {
        var programs = await _repository.GetAllAsync();
        return programs.Select(MaptoDto).ToList();
    }

    public async Task<ProgramDto?> GetByIdAsync(Guid id)
    {
        var program = await _repository.GetByIdAsync(id);
        return program == null ? null : MaptoDto(program);
    }

    public async Task<List<ProgramDto>> GetByFacultyAsync(string faculty)
    {
        var programs = await _repository.GetAllAsync();
        return programs
            .Where(p => p.Faculty == faculty)
            .Select(MaptoDto)
            .ToList();
    }

    public async Task<List<ProgramDto>> GetByDegreeAsync(string degree)
    {
        var programs = await _repository.GetAllAsync();
        return programs
            .Where(p => p.Degree == degree)
            .Select(MaptoDto)
            .ToList();
    }

    public async Task SyncProgramsAsync()
    {
        await Task.CompletedTask;
    }
}
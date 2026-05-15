using ProgramService.Application.DTOs;
using ProgramService.Domain.Entities;
using ProgramService.Application.Interfaces;

namespace ProgramService.Infrastructure.Services;

public class StudyProgramService : IStudyProgramService
{
    private readonly IStudyProgramRepository _repository;

    public StudyProgramService(IStudyProgramRepository repository)
    {
        _repository = repository;
    }

    private ProgramDto MapToDto(StudyProgram program)
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
        return programs.Select(MapToDto).ToList();
    }

    public async Task<ProgramDto?> GetByIdAsync(Guid id)
    {
        var program = await _repository.GetByIdAsync(id);
        return program == null ? null : MapToDto(program);
    }

    public async Task<List<ProgramDto>> GetByFacultyAsync(string faculty)
    {
        var programs = await _repository.GetAllAsync();
        return programs
            .Where(p => p.Faculty == faculty)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<ProgramDto>> GetByDegreeAsync(string degree)
    {
        var programs = await _repository.GetAllAsync();
        return programs
            .Where(p => p.Degree == degree)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<ProgramDto>> SearchAsync(string? faculty, string? degree, int page, int pageSize)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1) 
            pageSize = 10;


        var programs = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(faculty))
        {
            programs = programs
                .Where(p => p.Faculty == faculty)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(degree))
        {
            programs = programs
                .Where(p => p.Degree == degree)
                .ToList();
        }

        return programs
            .Skip((page - 1) * pageSize)
            .TakeLast(pageSize)
            .Select(MapToDto)
            .ToList();
    }

    public async Task SyncProgramsAsync()
    {
        var externalPrograms = new List<ExternalProgramDto> // имитация внешней системы
        {
            new ExternalProgramDto
            {
                ExternalId = "ext-1",
                Title = "Computer Science",
                Description = "CS program",
                BudgetPlaces = 50,
                PaidPlaces = 20,
                Faculty = "IT",
                Duration = 4,
                Degree = "Bachelor"
            },
            new ExternalProgramDto
            {
                ExternalId = "ext-2",
                Title = "Economics",
                Description = "Economics program",
                BudgetPlaces = 30,
                PaidPlaces = 40,
                Faculty = "Economics",
                Duration = 4,
                Degree = "Bachelor"
            }
        };

        foreach (var ext in externalPrograms)
        {
            var existing = await _repository.GetByExternalIdAsync(ext.ExternalId);

            if (existing != null)  // обновление
            {
                existing.Title = ext.Title;
                existing.Description = ext.Description;
                existing.BudgetPlaces = ext.BudgetPlaces;
                existing.PaidPlaces = ext.PaidPlaces;
                existing.Faculty = ext.Faculty;
                existing.Duration = ext.Duration;
                existing.Degree = ext.Degree;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existing);
            }
            else  // создание
            {
                var newProgram = new StudyProgram
                {
                    Id = Guid.NewGuid(),
                    ExternalId = ext.ExternalId,
                    Title = ext.Title,
                    Description = ext.Description,
                    BudgetPlaces = ext.BudgetPlaces,
                    PaidPlaces = ext.PaidPlaces,
                    Faculty = ext.Faculty,
                    Duration = ext.Duration,
                    Degree = ext.Degree,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.CreateAsync(newProgram);
            }
        }
    }
}
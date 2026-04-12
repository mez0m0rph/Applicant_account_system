using ProgramService.Application.DTOs;

namespace ProgramService.Application.Interfaces;

public interface IStudyProgramService
{
    Task<List<ProgramDto>> GetAllAsync();
    Task<ProgramDto?> GetByIdAsync(Guid id);
    Task SyncProgramsAsync();
    Task<List<ProgramDto>> GetByFacultyAsync(string faculty);
    Task<List<ProgramDto>> GetByDegreeAsync(string degree);
    Task<List<ProgramDto>> SearchAsync(string? faculty, string? degree, int page, int pageSize);
}
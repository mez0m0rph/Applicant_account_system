using ProgramService.Repositories;
using ProgramService.Models;
using ProgramService.DTOs;

namespace ProgramService.Services;

public interface IStudyProgramService
{
    Task<List<ProgramDto>> GetAllAsync();
    Task<ProgramDto?> GetByIdAsync(Guid id);
    Task SyncProgramsAsync();
    Task<List<ProgramDto>> GetByFacultyAsync(string faculty);
    Task<List<ProgramDto>> GetByDegreeAsync(string degree);
}
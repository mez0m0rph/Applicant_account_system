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

    public async Task<List<ProgramDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}
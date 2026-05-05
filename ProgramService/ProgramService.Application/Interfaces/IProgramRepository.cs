using ProgramService.Domain.Entities;

namespace ProgramService.Application.Interfaces;

public interface IProgramRepository
{
    Task<List<StudyProgram>> GetAllAsync();
    Task<StudyProgram?> GetByExternalIdAsync(string externalId);
    Task CreateAsync(StudyProgram program);
    Task UpdateAsync(StudyProgram program);
}

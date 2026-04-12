using ProgramService.Domain.Entities;

namespace ProgramService.Application.Interfaces;

public interface IStudyProgramRepository
{
    Task<List<StudyProgram>> GetAllAsync();
    Task<StudyProgram?> GetByIdAsync(Guid id);
    Task<StudyProgram?> GetByExternalIdAsync(string externalId);
    Task CreateAsync(StudyProgram program);
    Task UpdateAsync(StudyProgram program);
    Task DeleteAsync(Guid id);
}
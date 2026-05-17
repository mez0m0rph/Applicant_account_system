using ProgramService.Application.DTOs;
using ProgramService.Domain.Entities;

namespace ProgramService.Application.Interfaces;

public interface IProgramRepository
{
    Task<List<StudyProgram>> GetAllAsync();
    Task<(List<StudyProgram> Items, int TotalCount)> GetPagedAsync(GetProgramsQuery query);
    Task<StudyProgram?> GetByIdAsync(Guid id);
    Task<StudyProgram?> GetByExternalIdAsync(string externalId);
    Task CreateAsync(StudyProgram program);
    Task UpdateAsync(StudyProgram program);
}

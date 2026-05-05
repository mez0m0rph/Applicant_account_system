namespace ProgramService.Application.Interfaces;

public interface IProgramImportService
{
    Task<int> ImportAsync();
}

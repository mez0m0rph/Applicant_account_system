namespace ProgramService.Application.DTOs.External;

public class ExternalProgramDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FacultyName { get; set; }
    public string? EducationLevel { get; set; }
    public string? EducationForm { get; set; }
    public string? Language { get; set; }
    public int BudgetPlaces { get; set; }
    public int PaidPlaces { get; set; }
    public int Duration { get; set; }
}

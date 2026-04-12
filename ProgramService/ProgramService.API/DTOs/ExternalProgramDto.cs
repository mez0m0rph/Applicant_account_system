namespace ProgramService.DTOs;

public class ExternalProgramDto
{
    public string ExternalId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int BudgetPlaces { get; set; }
    public int PaidPlaces { get; set; }
    public string Faculty { get; set; } = null!;
    public int Duration { get; set; }
    public string Degree { get; set; } = null!;
}
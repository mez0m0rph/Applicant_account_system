namespace WebApp.Models.Program;

public class StudyProgramViewModel
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BudgetPlaces { get; set; }
    public int PaidPlaces { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Degree { get; set; } = string.Empty;
}

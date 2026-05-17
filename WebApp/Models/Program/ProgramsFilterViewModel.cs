namespace WebApp.Models.Program;

public class ProgramsFilterViewModel
{
    public string? Search { get; set; }
    public string? Faculty { get; set; }
    public string? EducationLevel { get; set; }
    public string? EducationForm { get; set; }
    public string? Language { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

namespace AdmissionService.Application.DTOs.External;

public class ProgramDetailsDto
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BudgetPlaces { get; set; }
    public int PaidPlaces { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Degree { get; set; } = string.Empty;
}

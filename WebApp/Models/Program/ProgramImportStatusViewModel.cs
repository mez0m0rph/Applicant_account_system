namespace WebApp.Models.Program;

public class ProgramImportStatusViewModel
{
    public string Status { get; set; } = "NeverStarted";
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int ImportedCount { get; set; }
    public string Message { get; set; } = string.Empty;
}

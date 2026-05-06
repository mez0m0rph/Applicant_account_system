namespace WebApp.Models.Admission;

public class AdmissionProgramViewModel
{
    public Guid ProgramId { get; set; }
    public int Priority { get; set; }

    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
}

namespace WebApp.Models.Program;

public class ProgramsIndexPageViewModel
{
    public List<ProgramViewModel> Programs { get; set; } = new();
    public HashSet<Guid> SelectedProgramIds { get; set; } = new();
}

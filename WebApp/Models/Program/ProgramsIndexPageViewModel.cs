namespace WebApp.Models.Program;

public class ProgramsIndexPageViewModel
{
    public PagedProgramsViewModel PagedPrograms { get; set; } = new();
    public ProgramsFilterViewModel Filter { get; set; } = new();
    public HashSet<Guid> SelectedProgramIds { get; set; } = new();
}

using WebApp.Models.Manager;
using WebApp.Models.Program;

namespace WebApp.Models.Staff;

public class StaffAdmissionsPageViewModel
{
    public PagedAdmissionsViewModel PagedAdmissions { get; set; } = new();
    public StaffAdmissionsFilterViewModel Filter { get; set; } = new();
    public List<ManagerViewModel> Managers { get; set; } = new();
    public List<ProgramViewModel> Programs { get; set; } = new();
}

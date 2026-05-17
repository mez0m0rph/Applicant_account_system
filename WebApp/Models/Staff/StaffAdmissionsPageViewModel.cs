using WebApp.Models.Manager;

namespace WebApp.Models.Staff;

public class StaffAdmissionsPageViewModel
{
    public PagedAdmissionsViewModel PagedAdmissions { get; set; } = new();
    public StaffAdmissionsFilterViewModel Filter { get; set; } = new();
    public List<ManagerViewModel> Managers { get; set; } = new();
}

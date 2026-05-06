using WebApp.Models.Admission;
using WebApp.Models.Manager;

namespace WebApp.Models.Staff;

public class StaffAdmissionsPageViewModel
{
    public List<AdmissionViewModel> Admissions { get; set; } = new();
    public List<ManagerViewModel> Managers { get; set; } = new();
}

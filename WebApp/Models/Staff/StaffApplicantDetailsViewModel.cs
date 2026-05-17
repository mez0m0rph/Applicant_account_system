using WebApp.Models.Admission;
using WebApp.Models.Applicant;
using WebApp.Models.Document;
using WebApp.Models.Program;

namespace WebApp.Models.Staff;

public class StaffApplicantDetailsViewModel
{
    public Guid ApplicantUserId { get; set; }

    public ProfileViewModel? Profile { get; set; }
    public AdmissionViewModel? Admission { get; set; }
    public List<DocumentViewModel> Documents { get; set; } = new();
    public List<ProgramViewModel> AvailablePrograms { get; set; } = new();
}

namespace WebApp.Models.Admission;

public class UpdateAdmissionStatusViewModel
{
    public Guid AdmissionId { get; set; }
    public string Status { get; set; } = string.Empty;
}

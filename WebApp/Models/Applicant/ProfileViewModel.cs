namespace WebApp.Models.Applicant;

public class ProfileViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; }
    public string Citizenship { get; set; } = string.Empty;

    public bool HasProfile { get; set; }
}

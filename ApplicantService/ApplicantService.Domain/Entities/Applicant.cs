using ApplicantService.Domain.Enums;

namespace ApplicantService.Domain.Entities;

public class Applicant
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public Gender Gender { get; set; }

    public string Citizenship { get; set; } = string.Empty;
}

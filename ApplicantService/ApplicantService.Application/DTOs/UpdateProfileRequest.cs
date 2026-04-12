namespace ApplicantService.Application.DTOs;

public class UpdateProfileRequest
{
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime BirthDate { get; set; } 
}
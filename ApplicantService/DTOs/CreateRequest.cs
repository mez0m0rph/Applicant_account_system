namespace ApplicantService.DTOs;

public class CreateRequest
{
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string UserId { get; set; } = null!;
}
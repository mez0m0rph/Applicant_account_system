namespace ApplicantService.DTOs;

public class CreateRequest
{
    public string UserId { get; set; } = null!;  // это айдишник пользователя из AuthService (JWT)
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime BirthDate { get; set; }
}
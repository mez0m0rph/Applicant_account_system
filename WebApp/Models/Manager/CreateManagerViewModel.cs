namespace WebApp.Models.Manager;

public class CreateManagerViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Manager";
    public string Faculty { get; set; } = string.Empty;
}

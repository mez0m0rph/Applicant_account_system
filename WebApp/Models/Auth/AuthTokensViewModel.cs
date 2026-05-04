namespace WebApp.Models.Auth;

public class AuthTokensViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
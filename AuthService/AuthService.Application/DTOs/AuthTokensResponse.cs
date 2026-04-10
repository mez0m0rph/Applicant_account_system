namespace AuthService.Application.DTOs;

public class AuthTokensResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

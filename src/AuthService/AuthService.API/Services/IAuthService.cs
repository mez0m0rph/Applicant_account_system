using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);
}
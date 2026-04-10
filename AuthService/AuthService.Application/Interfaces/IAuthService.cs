using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthTokensResponse> LoginAsync(LoginRequest request);
    Task<AuthTokensResponse> RefreshAsync(RefreshTokenRequest request);
    Task LogoutAsync(LogoutRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId);
}

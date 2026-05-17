using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<Guid> CreateStaffAsync(CreateStaffRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task DeleteUserAsync(Guid userId);
}

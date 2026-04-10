using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;

namespace AuthService.Infrastructure.Services;

public class AuthServiceImpl : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;

    public AuthServiceImpl(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Пользователь с таким email уже существует");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Applicant
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<AuthTokensResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Такой пользователь не найден");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new Exception("Неверный пароль");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthTokensResponse> RefreshAsync(RefreshTokenRequest request)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingRefreshToken == null)
            throw new Exception("Refresh token не найден");

        if (existingRefreshToken.IsRevoked)
            throw new Exception("Refresh token уже отозван");

        if (existingRefreshToken.ExpiresAtUtc <= DateTime.UtcNow)
            throw new Exception("Refresh token истек");

        existingRefreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

        return await IssueTokensAsync(existingRefreshToken.User);
    }

    public async Task LogoutAsync(LogoutRequest request)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingRefreshToken == null)
            return;

        if (!existingRefreshToken.IsRevoked)
        {
            existingRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);
        }
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isPasswordValid)
            throw new Exception("Текущий пароль неверный");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        return new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private async Task<AuthTokensResponse> IssueTokensAsync(User user)
    {
        var accessToken = _jwtService.GenerateToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            User = user,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthTokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue
        };
    }
}

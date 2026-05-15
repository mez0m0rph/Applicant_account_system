using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using Shared.Contracts.Events;
using Shared.Messaging.Interfaces;

namespace AuthService.Infrastructure.Services;

public class AuthServiceImpl : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMessagePublisher _messagePublisher;

    public AuthServiceImpl(
        IUserRepository userRepository,
        IJwtService jwtService,
        IMessagePublisher messagePublisher)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _messagePublisher = messagePublisher;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Пользователь с таким email уже существует");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Applicant
        };

        await _userRepository.CreateAsync(user);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Subject = "Регистрация завершена",
            Message = "Ваш аккаунт успешно создан."
        });
    }

    public async Task<Guid> CreateStaffAsync(CreateStaffRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Пользователь с таким email уже существует");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
            throw new Exception("Некорректная роль");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = parsedRole
        };

        await _userRepository.CreateAsync(user);

        await _messagePublisher.PublishAsync(new NotificationRequestedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Subject = "Создан аккаунт сотрудника",
            Message = $"Для вас создан аккаунт с ролью {parsedRole}. Используйте указанные учетные данные для входа."
        });

        return user.Id;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Неверный email или пароль");

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null)
            throw new Exception("Неверный refresh token");

        if (user.RefreshTokenExpiresAt == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            throw new Exception("Refresh token истек");

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new Exception("Текущий пароль введен неверно");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }
}

using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;

namespace AuthService.Services;

public class AuthServiceImpl : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthServiceImpl(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new Exception("Пользователь с таким email уже существует");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = "Applicant"
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser == null)
        {
            throw new Exception("Такой пользователь не найден");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            existingUser.PasswordHash
        );

        if (!isPasswordValid)
        {
            throw new Exception("Неверный пароль");
        }

        var token = _jwtService.GenerateToken(existingUser);

        return new LoginResponse
        {
            Token = token
        };
    }
}
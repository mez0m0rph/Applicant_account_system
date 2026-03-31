using ApplicantService.DTOs;
using ApplicantService.Models;
using ApplicantService.Repositories;
using System.Security.Claims;

namespace ApplicantService.Services;

public class ApplicantServiceImpl : IApplicantService
{
    private readonly IApplicantRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicantServiceImpl(IApplicantRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserIdFromToken()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new Exception("Пользователь не авторизован");
        return userId;
    }

    public async Task CreateAsync(CreateRequest request)
    {
        var userId = Guid.Parse(GetUserIdFromToken());
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile != null)
            throw new Exception("Профиль уже существует");

        var applicant = new Applicant
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName,
            Phone = request.Phone,
            BirthDate = request.BirthDate
        };

        await _repository.CreateAsync(applicant);
    }

    public async Task LoginAsync(LoginRequest request)
    {
        var userId = Guid.Parse(GetUserIdFromToken());
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile == null)
            throw new Exception("Профиль не найден");
    }
}
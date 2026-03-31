using ApplicantService.Repositories;
using ApplicantService.DTOs;
using ApplicantService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApplicantService.Services;

public class ApplicantService : IApplicantService
{
    private readonly IApplicantRepository _repository;
    public ApplicantService(IApplicantRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(CreateRequest request)
    {
        var userId = Guid.Parse(request.UserId);
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile != null)
        {
            throw new Exception("Профиль уже существует");
        }

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
        var userId = Guid.Parse(request.UserId);
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile == null)
        {
            throw new Exception("Профиль не найден");
        }

    }
}
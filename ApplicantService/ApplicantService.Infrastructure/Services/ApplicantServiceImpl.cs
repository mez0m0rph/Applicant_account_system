using ApplicantService.Application.DTOs;
using ApplicantService.Application.Interfaces;
using ApplicantService.Domain.Entities;

namespace ApplicantService.Infrastructure.Services;

public class ApplicantServiceImpl : IApplicantService
{
    private readonly IApplicantRepository _repository;

    public ApplicantServiceImpl(IApplicantRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(Guid userId, CreateRequest request)
    {
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

    public async Task<GetProfileResponse> GetMyProfileAsync(Guid userId)
    {
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile == null)
            throw new Exception("Профиль не был создан ранее");

        return new GetProfileResponse
        {
            FullName = existingProfile.FullName,
            Phone = existingProfile.Phone,
            BirthDate = existingProfile.BirthDate
        };
    }

    public async Task UpdateAsync(Guid userId, UpdateProfileRequest request)
    {
        var existingProfile = await _repository.GetByUserIdAsync(userId);

        if (existingProfile == null)
            throw new Exception("Профиль не был создан ранее");

        existingProfile.FullName = request.FullName;
        existingProfile.Phone = request.Phone;
        existingProfile.BirthDate = request.BirthDate;

        await _repository.UpdateAsync(existingProfile);
    }
}
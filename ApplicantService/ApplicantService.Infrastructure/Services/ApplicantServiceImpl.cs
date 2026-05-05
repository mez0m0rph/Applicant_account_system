using ApplicantService.Application.DTOs;
using ApplicantService.Application.Interfaces;
using ApplicantService.Domain.Entities;
using ApplicantService.Domain.Enums;

namespace ApplicantService.Infrastructure.Services;

public class ApplicantServiceImpl : IApplicantService
{
    private readonly IApplicantRepository _repository;

    public ApplicantServiceImpl(IApplicantRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(Guid userId, CreateApplicantRequest request)
    {
        var existing = await _repository.GetByUserIdAsync(userId);
        if (existing != null)
            throw new Exception("Профиль уже существует");

        var applicant = new Applicant
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            BirthDate = request.BirthDate,
            Gender = (Gender)request.Gender,
            Citizenship = request.Citizenship
        };

        await _repository.CreateAsync(applicant);
    }

    public async Task<ApplicantResponse?> GetMyAsync(Guid userId)
    {
        var applicant = await _repository.GetByUserIdAsync(userId);
        if (applicant == null)
            return null;

        return new ApplicantResponse
        {
            Id = applicant.Id,
            UserId = applicant.UserId,
            FullName = applicant.FullName,
            Email = applicant.Email,
            Phone = applicant.Phone,
            BirthDate = applicant.BirthDate,
            Gender = applicant.Gender.ToString(),
            Citizenship = applicant.Citizenship
        };
    }

    public async Task UpdateAsync(Guid userId, UpdateApplicantRequest request)
    {
        var applicant = await _repository.GetByUserIdAsync(userId);
        if (applicant == null)
            throw new Exception("Профиль не найден");

        applicant.FullName = request.FullName;
        applicant.Email = request.Email;
        applicant.Phone = request.Phone;
        applicant.BirthDate = request.BirthDate;
        applicant.Gender = (Gender)request.Gender;
        applicant.Citizenship = request.Citizenship;

        await _repository.UpdateAsync(applicant);
    }
}

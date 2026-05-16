using ApplicantService.Application.DTOs;
using ApplicantService.Application.Interfaces;
using ApplicantService.Domain.Entities;
using ApplicantService.Domain.Enums;

namespace ApplicantService.Infrastructure.Services;

public class ApplicantServiceImpl : IApplicantService
{
    private readonly IApplicantRepository _repository;
    private readonly IAdmissionCatalogClient _admissionCatalogClient;

    public ApplicantServiceImpl(
        IApplicantRepository repository,
        IAdmissionCatalogClient admissionCatalogClient)
    {
        _repository = repository;
        _admissionCatalogClient = admissionCatalogClient;
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private async Task EnsureApplicantEditableAsync(Guid userId)
    {
        var admission = await _admissionCatalogClient.GetMyAsync(userId);

        if (admission != null &&
            string.Equals(admission.Status, "Closed", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Нельзя изменять личные данные, когда заявление закрыто");
        }
    }

    public async Task CreateAsync(Guid userId, CreateApplicantRequest request)
    {
        await EnsureApplicantEditableAsync(userId);

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
            BirthDate = NormalizeUtc(request.BirthDate),
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
            Gender = (int)applicant.Gender,
            Citizenship = applicant.Citizenship
        };
    }

    public async Task UpdateAsync(Guid userId, UpdateApplicantRequest request)
    {
        await EnsureApplicantEditableAsync(userId);

        var applicant = await _repository.GetByUserIdAsync(userId);

        if (applicant == null)
        {
            applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                BirthDate = NormalizeUtc(request.BirthDate),
                Gender = (Gender)request.Gender,
                Citizenship = request.Citizenship
            };

            await _repository.CreateAsync(applicant);
            return;
        }

        applicant.FullName = request.FullName;
        applicant.Email = request.Email;
        applicant.Phone = request.Phone;
        applicant.BirthDate = NormalizeUtc(request.BirthDate);
        applicant.Gender = (Gender)request.Gender;
        applicant.Citizenship = request.Citizenship;

        await _repository.UpdateAsync(applicant);
    }
}
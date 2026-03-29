using ApplicantService.DTOs;

namespace ApplicantService.Services;

// функционал: получить профиль -> если есть - вернуть/ нет - создать -> обновление профиля
public interface IApplicantService
{
    Task CreateAsync(CreateRequest request);

    Task LoginAsync(LoginRequest request);
}
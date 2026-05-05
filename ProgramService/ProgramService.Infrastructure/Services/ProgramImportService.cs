using System.Net.Http.Headers;
using System.Net.Http.Json;
using ProgramService.Application.DTOs.External;
using ProgramService.Application.Interfaces;
using ProgramService.Domain.Entities;

namespace ProgramService.Infrastructure.Services;

public class ProgramImportService : IProgramImportService
{
    private readonly HttpClient _httpClient;
    private readonly IProgramRepository _repository;

    public ProgramImportService(HttpClient httpClient, IProgramRepository repository)
    {
        _httpClient = httpClient;
        _repository = repository;
    }

    public async Task<int> ImportAsync()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://1c-mockup.kreosoft.space/api/programs");

        var credentials = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes("student:ny6gQnyn4ecbBrP9l1Fz"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<ExternalProgramDto>>() ?? new();
        var count = 0;

        foreach (var item in items)
        {
            var existing = await _repository.GetByExternalIdAsync(item.Id);

            if (existing == null)
            {
                var program = new StudyProgram
                {
                    Id = Guid.NewGuid(),
                    ExternalId = item.Id,
                    Code = item.Code,
                    Title = item.Name,
                    Description = item.Description ?? string.Empty,
                    BudgetPlaces = item.BudgetPlaces,
                    PaidPlaces = item.PaidPlaces,
                    Faculty = item.FacultyName ?? string.Empty,
                    EducationLevel = item.EducationLevel ?? string.Empty,
                    EducationForm = item.EducationForm ?? string.Empty,
                    Language = item.Language ?? string.Empty,
                    Duration = item.Duration,
                    Degree = item.EducationLevel ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.CreateAsync(program);
            }
            else
            {
                existing.Code = item.Code;
                existing.Title = item.Name;
                existing.Description = item.Description ?? string.Empty;
                existing.BudgetPlaces = item.BudgetPlaces;
                existing.PaidPlaces = item.PaidPlaces;
                existing.Faculty = item.FacultyName ?? string.Empty;
                existing.EducationLevel = item.EducationLevel ?? string.Empty;
                existing.EducationForm = item.EducationForm ?? string.Empty;
                existing.Language = item.Language ?? string.Empty;
                existing.Duration = item.Duration;
                existing.Degree = item.EducationLevel ?? string.Empty;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existing);
            }

            count++;
        }

        return count;
    }
}

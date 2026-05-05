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
            "https://1c-mockup.kreosoft.space/api/dictionary/programs");

        var credentials = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes("student:ny6gQnyn4ecbBrP9l1Fz"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ExternalProgramsResponse>();
        var items = payload?.Programs ?? new List<ExternalProgramDto>();

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
                    Description = string.Empty,
                    BudgetPlaces = 0,
                    PaidPlaces = 0,
                    Faculty = item.Faculty?.Name ?? string.Empty,
                    EducationLevel = item.EducationLevel?.Name ?? string.Empty,
                    EducationForm = item.EducationForm ?? string.Empty,
                    Language = item.Language ?? string.Empty,
                    Duration = 0,
                    Degree = item.EducationLevel?.Name ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.CreateAsync(program);
            }
            else
            {
                existing.Code = item.Code;
                existing.Title = item.Name;
                existing.Faculty = item.Faculty?.Name ?? string.Empty;
                existing.EducationLevel = item.EducationLevel?.Name ?? string.Empty;
                existing.EducationForm = item.EducationForm ?? string.Empty;
                existing.Language = item.Language ?? string.Empty;
                existing.Degree = item.EducationLevel?.Name ?? string.Empty;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existing);
            }

            count++;
        }

        return count;
    }
}

using System.Net.Http.Json;
using AdmissionService.Application.DTOs.External;
using AdmissionService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AdmissionService.Infrastructure.Services;

public class ApplicantCatalogClient : IApplicantCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ApplicantCatalogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApplicantProfileDto?> GetByUserIdAsync(Guid userId)
    {
        var baseUrl = _configuration["ApiUrls:Applicant"];
        var response = await _httpClient.GetAsync($"{baseUrl}/applicant/{userId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ApplicantProfileDto>();
    }
}

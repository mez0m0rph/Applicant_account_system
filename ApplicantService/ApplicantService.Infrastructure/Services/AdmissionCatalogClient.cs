using System.Net.Http.Json;
using ApplicantService.Application.DTOs.External;
using ApplicantService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ApplicantService.Infrastructure.Services;

public class AdmissionCatalogClient : IAdmissionCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AdmissionCatalogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AdmissionDetailsDto?> GetMyAsync(Guid applicantUserId)
    {
        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions/applicant/{applicantUserId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AdmissionDetailsDto>();
    }
}
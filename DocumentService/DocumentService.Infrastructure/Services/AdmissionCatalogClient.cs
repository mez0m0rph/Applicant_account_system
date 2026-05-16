using System.Net.Http.Json;
using DocumentService.Application.DTOs.External;
using DocumentService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DocumentService.Infrastructure.Services;

public class AdmissionCatalogClient : IAdmissionCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AdmissionCatalogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AdmissionDetailsDto?> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions/applicant/{applicantUserId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AdmissionDetailsDto>();
    }
}
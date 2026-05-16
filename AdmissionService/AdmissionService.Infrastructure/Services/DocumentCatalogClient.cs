using System.Net.Http.Json;
using AdmissionService.Application.DTOs.External;
using AdmissionService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AdmissionService.Infrastructure.Services;

public class DocumentCatalogClient : IDocumentCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public DocumentCatalogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<DocumentDetailsDto>> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.GetAsync($"{baseUrl}/documents/applicant/{applicantUserId}");

        if (!response.IsSuccessStatusCode)
            return new List<DocumentDetailsDto>();

        return await response.Content.ReadFromJsonAsync<List<DocumentDetailsDto>>() ?? new List<DocumentDetailsDto>();
    }
}

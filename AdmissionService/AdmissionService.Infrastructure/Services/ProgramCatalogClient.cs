using System.Net.Http.Json;
using AdmissionService.Application.DTOs.External;
using AdmissionService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AdmissionService.Infrastructure.Services;

public class ProgramCatalogClient : IProgramCatalogClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProgramCatalogClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ProgramDetailsDto?> GetByIdAsync(Guid programId)
    {
        var baseUrl = _configuration["ApiUrls:Program"];
        var response = await _httpClient.GetAsync($"{baseUrl}/programs/{programId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ProgramDetailsDto>();
    }
}

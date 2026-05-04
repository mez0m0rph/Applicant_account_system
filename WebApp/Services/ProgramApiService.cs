using System.Net.Http.Json;
using WebApp.Models.Common;
using WebApp.Models.Program;

namespace WebApp.Services;

public class ProgramApiService : IProgramApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProgramApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ApiResult<List<StudyProgramViewModel>>> GetAllAsync()
    {
        var baseUrl = _configuration["ApiUrls:Program"];
        var response = await _httpClient.GetAsync($"{baseUrl}/programs");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<StudyProgramViewModel>>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<List<StudyProgramViewModel>>();
        return ApiResult<List<StudyProgramViewModel>>.Ok(data ?? new List<StudyProgramViewModel>());
    }
}

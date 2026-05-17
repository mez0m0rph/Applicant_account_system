using System.Net.Http.Json;
using System.Text;
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

    public async Task<ApiResult<PagedProgramsViewModel>> GetAllAsync(ProgramsFilterViewModel filter)
    {
        var baseUrl = _configuration["ApiUrls:Program"];

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query.Add($"search={Uri.EscapeDataString(filter.Search)}");

        if (!string.IsNullOrWhiteSpace(filter.Faculty))
            query.Add($"faculty={Uri.EscapeDataString(filter.Faculty)}");

        if (!string.IsNullOrWhiteSpace(filter.EducationLevel))
            query.Add($"educationLevel={Uri.EscapeDataString(filter.EducationLevel)}");

        if (!string.IsNullOrWhiteSpace(filter.EducationForm))
            query.Add($"educationForm={Uri.EscapeDataString(filter.EducationForm)}");

        if (!string.IsNullOrWhiteSpace(filter.Language))
            query.Add($"language={Uri.EscapeDataString(filter.Language)}");

        query.Add($"page={filter.Page}");
        query.Add($"pageSize={filter.PageSize}");

        var url = new StringBuilder($"{baseUrl}/programs");
        if (query.Count > 0)
            url.Append('?').Append(string.Join("&", query));

        var response = await _httpClient.GetAsync(url.ToString());

        if (!response.IsSuccessStatusCode)
            return ApiResult<PagedProgramsViewModel>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<PagedProgramsViewModel>();
        return ApiResult<PagedProgramsViewModel>.Ok(data ?? new PagedProgramsViewModel());
    }
}

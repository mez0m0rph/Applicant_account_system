using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebApp.Models.Common;
using WebApp.Models.Program;

namespace WebApp.Services;

public class ProgramApiService : IProgramApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProgramApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<ApiResult<string>> ImportCatalogsAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Program"];

        var response = await _httpClient.PostAsync($"{baseUrl}/catalog/import", null);

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Импорт справочников завершен"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка импорта справочников"));
    }

    public async Task<ApiResult<ProgramImportStatusViewModel>> GetImportStatusAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Program"];

        var response = await _httpClient.GetAsync($"{baseUrl}/catalog/import/status");

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return ApiResult<ProgramImportStatusViewModel>.Fail(
                ReadMessage(content, "Ошибка получения статуса импорта"));

        var data = await response.Content.ReadFromJsonAsync<ProgramImportStatusViewModel>();

        return ApiResult<ProgramImportStatusViewModel>.Ok(data ?? new ProgramImportStatusViewModel());
    }

    private static string ReadMessage(string? content, string fallback)
    {
        if (string.IsNullOrWhiteSpace(content))
            return fallback;

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("error", out var errorProp))
                    return errorProp.GetString() ?? fallback;

                if (root.TryGetProperty("message", out var messageProp))
                    return messageProp.GetString() ?? fallback;

                if (root.TryGetProperty("importedPrograms", out var importedProgramsProp))
                    return $"Импортировано программ: {importedProgramsProp.GetInt32()}";

                if (root.TryGetProperty("imported", out var importedProp))
                    return $"Импортировано записей: {importedProp.GetInt32()}";
            }
        }
        catch
        {
        }

        return content;
    }
}
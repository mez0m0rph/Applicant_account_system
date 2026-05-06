using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApp.Models.Admission;
using WebApp.Models.Common;

namespace WebApp.Services;

public class AdmissionApiService : IAdmissionApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdmissionApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<string>> CreateAsync(CreateAdmissionViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsync($"{baseUrl}/admissions", null);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Заявление успешно создано"))
            : ApiResult<string>.Fail(ReadMessage(content, $"Ошибка создания заявления. HTTP {(int)response.StatusCode}"));
    }

    public async Task<ApiResult<AdmissionViewModel>> GetMyAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions/my");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return ApiResult<AdmissionViewModel>.Ok(null!);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return ApiResult<AdmissionViewModel>.Fail(
                ReadMessage(error, $"Ошибка получения заявления. HTTP {(int)response.StatusCode}"));
        }

        var data = await response.Content.ReadFromJsonAsync<AdmissionViewModel>();
        return data == null
            ? ApiResult<AdmissionViewModel>.Fail("Пустой ответ")
            : ApiResult<AdmissionViewModel>.Ok(data);
    }

    public async Task<ApiResult<List<AdmissionViewModel>>> GetAllAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return ApiResult<List<AdmissionViewModel>>.Fail(
                ReadMessage(error, $"Ошибка получения списка заявлений. HTTP {(int)response.StatusCode}"));
        }

        var data = await response.Content.ReadFromJsonAsync<List<AdmissionViewModel>>();
        return ApiResult<List<AdmissionViewModel>>.Ok(data ?? new());
    }

    public async Task<ApiResult<string>> AddProgramAsync(Guid programId, int priority)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/admissions/my/programs", new
        {
            programId,
            priority
        });

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Программа добавлена в заявление"))
            : ApiResult<string>.Fail(ReadMessage(content, $"Ошибка добавления программы. HTTP {(int)response.StatusCode}"));
    }

    public async Task<ApiResult<string>> UpdateProgramPriorityAsync(Guid programId, int priority)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/admissions/my/programs/{programId}/priority", new
        {
            priority
        });

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Приоритет обновлен"))
            : ApiResult<string>.Fail(ReadMessage(content, $"Ошибка изменения приоритета. HTTP {(int)response.StatusCode}"));
    }

    public async Task<ApiResult<string>> RemoveProgramAsync(Guid programId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.DeleteAsync($"{baseUrl}/admissions/my/programs/{programId}");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Программа удалена из заявления"))
            : ApiResult<string>.Fail(ReadMessage(content, $"Ошибка удаления программы. HTTP {(int)response.StatusCode}"));
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

                if (root.TryGetProperty("title", out var titleProp))
                    return titleProp.GetString() ?? fallback;
            }
        }
        catch
        {
        }

        return content;
    }
}

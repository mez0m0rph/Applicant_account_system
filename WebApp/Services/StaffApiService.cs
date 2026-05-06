using System.Net.Http.Json;
using System.Text.Json;
using WebApp.Models.Admission;
using WebApp.Models.Common;
using WebApp.Models.Manager;

namespace WebApp.Services;

public class StaffApiService : IStaffApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StaffApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<string>> CreateManagerAsync(CreateManagerViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var authBaseUrl = _configuration["ApiUrls:Auth"];
        var managerBaseUrl = _configuration["ApiUrls:Manager"];

        var createUserPayload = new
        {
            email = model.Email,
            password = model.Password,
            role = model.Role
        };

        var authResponse = await _httpClient.PostAsJsonAsync($"{authBaseUrl}/auth/staff", createUserPayload);
        var authContent = await authResponse.Content.ReadAsStringAsync();

        if (!authResponse.IsSuccessStatusCode)
            return ApiResult<string>.Fail(ReadMessage(authContent, "Ошибка создания пользователя staff"));

        using var authJson = JsonDocument.Parse(authContent);
        var userId = authJson.RootElement.GetProperty("userId").GetGuid();

        var managerPayload = new
        {
            userId,
            fullName = model.FullName,
            email = model.Email,
            role = model.Role,
            faculty = model.Faculty
        };

        var managerResponse = await _httpClient.PostAsJsonAsync($"{managerBaseUrl}/managers", managerPayload);
        var managerContent = await managerResponse.Content.ReadAsStringAsync();

        return managerResponse.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(managerContent, "Менеджер создан"))
            : ApiResult<string>.Fail(ReadMessage(managerContent, "Ошибка создания менеджера"));
    }

    public async Task<ApiResult<List<ManagerViewModel>>> GetManagersAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Manager"];
        var response = await _httpClient.GetAsync($"{baseUrl}/managers");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<ManagerViewModel>>.Fail(ReadMessage(await response.Content.ReadAsStringAsync(), "Ошибка получения списка менеджеров"));

        var data = await response.Content.ReadFromJsonAsync<List<ManagerViewModel>>();
        return ApiResult<List<ManagerViewModel>>.Ok(data ?? new());
    }

    public async Task<ApiResult<ManagerViewModel>> GetManagerByIdAsync(Guid id)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Manager"];
        var response = await _httpClient.GetAsync($"{baseUrl}/managers/{id}");

        if (!response.IsSuccessStatusCode)
            return ApiResult<ManagerViewModel>.Fail(ReadMessage(await response.Content.ReadAsStringAsync(), "Ошибка получения менеджера"));

        var data = await response.Content.ReadFromJsonAsync<ManagerViewModel>();
        return data == null
            ? ApiResult<ManagerViewModel>.Fail("Пустой ответ")
            : ApiResult<ManagerViewModel>.Ok(data);
    }

    public async Task<ApiResult<string>> UpdateManagerAsync(EditManagerViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Manager"];
        var payload = new
        {
            userId = model.UserId,
            fullName = model.FullName,
            email = model.Email,
            role = model.Role,
            faculty = model.Faculty
        };

        var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/managers/{model.Id}", payload);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Менеджер обновлен"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка обновления менеджера"));
    }

    public async Task<ApiResult<string>> DeleteManagerAsync(Guid id)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Manager"];
        var response = await _httpClient.DeleteAsync($"{baseUrl}/managers/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Менеджер удален"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка удаления менеджера"));
    }

    public async Task<ApiResult<List<AdmissionViewModel>>> GetAdmissionsAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<AdmissionViewModel>>.Fail(ReadMessage(await response.Content.ReadAsStringAsync(), "Ошибка получения списка заявлений"));

        var data = await response.Content.ReadFromJsonAsync<List<AdmissionViewModel>>();
        return ApiResult<List<AdmissionViewModel>>.Ok(data ?? new());
    }

    public async Task<ApiResult<string>> AssignManagerAsync(AssignManagerViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var managersResult = await GetManagersAsync();
        if (!managersResult.Success)
            return ApiResult<string>.Fail(managersResult.Error ?? "Ошибка получения списка менеджеров");

        var manager = managersResult.Data?.FirstOrDefault(x => x.UserId == model.ManagerUserId);
        if (manager == null)
            return ApiResult<string>.Fail("Менеджер не найден");

        var baseUrl = _configuration["ApiUrls:Admission"];
        var payload = new
        {
            managerUserId = model.ManagerUserId,
            managerEmail = manager.Email
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/admissions/{model.AdmissionId}/assign-manager",
            payload);

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Менеджер назначен"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка назначения менеджера"));
    }

    public async Task<ApiResult<string>> ReleaseManagerAsync(Guid admissionId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsync($"{baseUrl}/admissions/{admissionId}/release-manager", null);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Менеджер снят"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка снятия менеджера"));
    }

    public async Task<ApiResult<string>> UpdateAdmissionStatusAsync(UpdateAdmissionStatusViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var payload = new { status = model.Status };

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/admissions/{model.AdmissionId}/status",
            payload);

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Статус обновлен"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка обновления статуса"));
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

                if (root.TryGetProperty("id", out _))
                    return fallback;
            }
        }
        catch
        {
        }

        return content;
    }
}

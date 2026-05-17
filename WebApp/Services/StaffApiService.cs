using System.Net.Http.Json;
using System.Text.Json;
using WebApp.Models.Admission;
using WebApp.Models.Common;
using WebApp.Models.Manager;
using WebApp.Models.Staff;

namespace WebApp.Services;

public class StaffApiService : IStaffApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StaffApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        var createStaffResponse = await _httpClient.PostAsJsonAsync($"{authBaseUrl}/auth/staff", new
        {
            email = model.Email,
            password = model.Password,
            role = model.Role
        });

        var createStaffContent = await createStaffResponse.Content.ReadAsStringAsync();
        if (!createStaffResponse.IsSuccessStatusCode)
            return ApiResult<string>.Fail(ReadMessage(createStaffContent, "Ошибка создания пользователя"));

        var userId = ExtractGuidFromContent(createStaffContent);
        if (userId == Guid.Empty)
            return ApiResult<string>.Fail($"Не удалось получить id созданного пользователя. Ответ AuthService: {createStaffContent}");

        var createManagerResponse = await _httpClient.PostAsJsonAsync($"{managerBaseUrl}/managers", new
        {
            userId = userId,
            fullName = model.FullName,
            email = model.Email,
            role = model.Role,
            faculty = model.Faculty
        });

        var createManagerContent = await createManagerResponse.Content.ReadAsStringAsync();

        return createManagerResponse.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(createManagerContent, "Менеджер создан"))
            : ApiResult<string>.Fail(ReadMessage(createManagerContent, "Ошибка создания менеджера"));
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

        var managerResult = await GetManagerByIdAsync(id);
        if (!managerResult.Success || managerResult.Data == null)
            return ApiResult<string>.Fail(managerResult.Error ?? "Менеджер не найден");

        var managerBaseUrl = _configuration["ApiUrls:Manager"];
        var authBaseUrl = _configuration["ApiUrls:Auth"];

        var deleteManagerResponse = await _httpClient.DeleteAsync($"{managerBaseUrl}/managers/{id}");
        var deleteManagerContent = await deleteManagerResponse.Content.ReadAsStringAsync();

        if (!deleteManagerResponse.IsSuccessStatusCode)
            return ApiResult<string>.Fail(ReadMessage(deleteManagerContent, "Ошибка удаления менеджера"));

        var deleteUserResponse = await _httpClient.DeleteAsync($"{authBaseUrl}/auth/users/{managerResult.Data.UserId}");
        var deleteUserContent = await deleteUserResponse.Content.ReadAsStringAsync();

        if (!deleteUserResponse.IsSuccessStatusCode)
            return ApiResult<string>.Fail(ReadMessage(deleteUserContent, "Менеджер удален, но не удалось удалить auth-пользователя"));

        return ApiResult<string>.Ok("Менеджер удален");
    }

    public async Task<ApiResult<PagedAdmissionsViewModel>> GetAdmissionsAsync(StaffAdmissionsFilterViewModel filter)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query.Add($"search={Uri.EscapeDataString(filter.Search)}");

        if (filter.ProgramId.HasValue)
            query.Add($"programId={filter.ProgramId.Value}");

        foreach (var faculty in filter.Faculties.Where(x => !string.IsNullOrWhiteSpace(x)))
            query.Add($"faculties={Uri.EscapeDataString(faculty)}");

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query.Add($"status={Uri.EscapeDataString(filter.Status)}");

        if (filter.OnlyUnassigned)
            query.Add("onlyUnassigned=true");

        if (filter.OnlyMine)
            query.Add("onlyMine=true");

        if (filter.AssignedManagerUserId.HasValue)
            query.Add($"assignedManagerUserId={filter.AssignedManagerUserId.Value}");

        query.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");
        query.Add($"sortDirection={Uri.EscapeDataString(filter.SortDirection)}");
        query.Add($"page={filter.Page}");
        query.Add($"pageSize={filter.PageSize}");

        var url = $"{baseUrl}/admissions";
        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return ApiResult<PagedAdmissionsViewModel>.Fail(ReadMessage(await response.Content.ReadAsStringAsync(), "Ошибка получения списка заявлений"));

        var data = await response.Content.ReadFromJsonAsync<PagedAdmissionsViewModel>();
        return ApiResult<PagedAdmissionsViewModel>.Ok(data ?? new PagedAdmissionsViewModel());
    }

    public async Task<ApiResult<string>> TakeAdmissionAsync(Guid admissionId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsync($"{baseUrl}/admissions/{admissionId}/take", null);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Поступление взято в работу"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка взятия поступления"));
    }

    public async Task<ApiResult<string>> ReleaseOwnAdmissionAsync(Guid admissionId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsync($"{baseUrl}/admissions/{admissionId}/release-own", null);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Поступление возвращено в общий пул"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка возврата поступления"));
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

        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/admissions/{model.AdmissionId}/assign-manager", payload);
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

        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/admissions/{model.AdmissionId}/status", payload);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(ReadMessage(content, "Статус обновлен"))
            : ApiResult<string>.Fail(ReadMessage(content, "Ошибка обновления статуса"));
    }

    private static Guid ExtractGuidFromContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Guid.Empty;

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in new[] { "id", "Id", "userId", "UserId" })
                {
                    if (root.TryGetProperty(key, out var prop))
                    {
                        if (prop.ValueKind == JsonValueKind.String && Guid.TryParse(prop.GetString(), out var parsedString))
                            return parsedString;

                        try
                        {
                            return prop.GetGuid();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        catch
        {
        }

        var tokens = content.Split(new[] { ' ', '\n', '\r', '\t', '"', '\'', '{', '}', ':', ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            if (Guid.TryParse(token, out var parsed))
                return parsed;
        }

        return Guid.Empty;
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

using System.Net.Http.Json;
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
            return ApiResult<string>.Fail(authContent);

        using var authJson = System.Text.Json.JsonDocument.Parse(authContent);
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
            ? ApiResult<string>.Ok(managerContent)
            : ApiResult<string>.Fail(managerContent);
    }

    public async Task<ApiResult<List<ManagerViewModel>>> GetManagersAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Manager"];
        var response = await _httpClient.GetAsync($"{baseUrl}/managers");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<ManagerViewModel>>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<List<ManagerViewModel>>();
        return ApiResult<List<ManagerViewModel>>.Ok(data ?? new());
    }

    public async Task<ApiResult<List<AdmissionViewModel>>> GetAdmissionsAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<AdmissionViewModel>>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<List<AdmissionViewModel>>();
        return ApiResult<List<AdmissionViewModel>>.Ok(data ?? new());
    }

    public async Task<ApiResult<string>> AssignManagerAsync(AssignManagerViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var payload = new
        {
            managerUserId = model.ManagerUserId,
            managerEmail = model.ManagerEmail
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/admissions/{model.AdmissionId}/assign-manager",
            payload);

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<string>> ReleaseManagerAsync(Guid admissionId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.PostAsync($"{baseUrl}/admissions/{admissionId}/release-manager", null);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
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
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }
}

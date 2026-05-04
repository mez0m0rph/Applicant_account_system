using System.Net.Http.Json;
using WebApp.Models.Applicant;
using WebApp.Models.Common;

namespace WebApp.Services;

public class ApplicantApiService : IApplicantApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicantApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<ProfileViewModel>> GetMyProfileAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Applicant"];
        var response = await _httpClient.GetAsync($"{baseUrl}/applicant/me");

        if (!response.IsSuccessStatusCode)
            return ApiResult<ProfileViewModel>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<ProfileViewModel>();
        return data == null
            ? ApiResult<ProfileViewModel>.Fail("Пустой ответ")
            : ApiResult<ProfileViewModel>.Ok(data);
    }

    public async Task<ApiResult<string>> CreateAsync(ProfileViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Applicant"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/applicant", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<string>> UpdateAsync(ProfileViewModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Applicant"];
        var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/applicant/me", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }
}

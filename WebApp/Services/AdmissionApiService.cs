using System.Net.Http.Json;
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
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/admissions", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<AdmissionViewModel>> GetMyAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Admission"];
        var response = await _httpClient.GetAsync($"{baseUrl}/admissions/my");

        if (!response.IsSuccessStatusCode)
            return ApiResult<AdmissionViewModel>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<AdmissionViewModel>();
        return data == null
            ? ApiResult<AdmissionViewModel>.Fail("Пустой ответ")
            : ApiResult<AdmissionViewModel>.Ok(data);
    }
}

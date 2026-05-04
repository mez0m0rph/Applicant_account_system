using System.Net.Http.Json;
using WebApp.Models.Common;
using WebApp.Models.Document;

namespace WebApp.Services;

public class DocumentApiService : IDocumentApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DocumentApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<string>> UploadAsync(UploadDocumentApiModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/documents", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<List<DocumentViewModel>>> GetMyAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.GetAsync($"{baseUrl}/documents/my");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<DocumentViewModel>>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<List<DocumentViewModel>>();
        return ApiResult<List<DocumentViewModel>>.Ok(data ?? new List<DocumentViewModel>());
    }
}

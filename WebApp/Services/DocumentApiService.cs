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

    public async Task<ApiResult<DownloadedFileViewModel>> DownloadAsync(Guid documentId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.GetAsync($"{baseUrl}/documents/my/{documentId}/download");

        if (!response.IsSuccessStatusCode)
            return ApiResult<DownloadedFileViewModel>.Fail(await response.Content.ReadAsStringAsync());

        var content = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        var fileName = "document";
        var disposition = response.Content.Headers.ContentDisposition;
        if (disposition != null)
        {
            fileName =
                disposition.FileNameStar ??
                disposition.FileName ??
                fileName;

            fileName = fileName.Trim('"');
        }

        return ApiResult<DownloadedFileViewModel>.Ok(new DownloadedFileViewModel
        {
            Content = content,
            ContentType = contentType,
            FileName = fileName
        });
    }

    public async Task<ApiResult<string>> DeleteAsync(Guid documentId)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.DeleteAsync($"{baseUrl}/documents/my/{documentId}");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<string>> UpdateAsync(Guid documentId, UpdateDocumentApiModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/documents/my/{documentId}", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    public async Task<ApiResult<string>> ReplaceFileAsync(Guid documentId, ReplaceDocumentFileApiModel model)
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Document"];
        var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/documents/my/{documentId}/file", model);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }
}

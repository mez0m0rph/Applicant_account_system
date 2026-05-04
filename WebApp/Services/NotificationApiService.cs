using System.Net.Http.Json;
using WebApp.Models.Common;
using WebApp.Models.Notification;

namespace WebApp.Services;

public class NotificationApiService : INotificationApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NotificationApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<List<NotificationViewModel>>> GetAllAsync()
    {
        ApiAuthHelper.ApplyBearerToken(_httpClient, _httpContextAccessor);

        var baseUrl = _configuration["ApiUrls:Notification"];
        var response = await _httpClient.GetAsync($"{baseUrl}/notifications");

        if (!response.IsSuccessStatusCode)
            return ApiResult<List<NotificationViewModel>>.Fail(await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<List<NotificationViewModel>>();
        return ApiResult<List<NotificationViewModel>>.Ok(data ?? new List<NotificationViewModel>());
    }
}

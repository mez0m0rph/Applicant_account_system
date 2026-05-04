using System.Net.Http.Headers;

namespace WebApp.Services;

public static class ApiAuthHelper
{
    public static void ApplyBearerToken(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        var token = httpContextAccessor.HttpContext?.Session.GetString("AccessToken");

        httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

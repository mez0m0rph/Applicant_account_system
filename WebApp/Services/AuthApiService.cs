using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebApp.Models.Auth;
using WebApp.Models.Common;

namespace WebApp.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<string>> LoginAsync(LoginViewModel model)
    {
        var baseUrl = _configuration["ApiUrls:Auth"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/auth/login", new
        {
            email = model.Email,
            password = model.Password
        });

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return ApiResult<string>.Fail(content);

        using var json = JsonDocument.Parse(content);

        var accessToken = json.RootElement.GetProperty("accessToken").GetString();
        var refreshToken = json.RootElement.GetProperty("refreshToken").GetString();

        if (string.IsNullOrWhiteSpace(accessToken))
            return ApiResult<string>.Fail("Access token не получен");

        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return ApiResult<string>.Fail("Session недоступна");

        session.SetString("AccessToken", accessToken);
        session.SetString("RefreshToken", refreshToken ?? string.Empty);
        session.SetString("UserEmail", model.Email);

        var payload = ReadJwtPayload(accessToken);

        var role = GetClaim(payload,
            "role",
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        var userId = GetClaim(payload,
            "nameid",
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        session.SetString("UserRole", role ?? string.Empty);
        session.SetString("UserId", userId ?? string.Empty);

        return ApiResult<string>.Ok("Успешный вход");
    }

    public async Task<ApiResult<string>> RegisterAsync(RegisterViewModel model)
    {
        var baseUrl = _configuration["ApiUrls:Auth"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/auth/register", new
        {
            email = model.Email,
            password = model.Password
        });

        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok(content)
            : ApiResult<string>.Fail(content);
    }

    private static Dictionary<string, string> ReadJwtPayload(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return new Dictionary<string, string>();

        var payload = parts[1]
            .Replace('-', '+')
            .Replace('_', '/');

        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }

        var bytes = Convert.FromBase64String(payload);
        var json = Encoding.UTF8.GetString(bytes);

        using var doc = JsonDocument.Parse(json);
        var result = new Dictionary<string, string>();

        foreach (var prop in doc.RootElement.EnumerateObject())
            result[prop.Name] = prop.Value.ToString();

        return result;
    }

    private static string? GetClaim(Dictionary<string, string> payload, params string[] names)
    {
        foreach (var name in names)
        {
            if (payload.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }
}

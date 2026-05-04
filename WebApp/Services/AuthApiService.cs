using System.Net.Http.Json;
using WebApp.Models.Auth;
using WebApp.Models.Common;

namespace WebApp.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
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

        if (!response.IsSuccessStatusCode)
            return ApiResult<string>.Fail(content);

        return ApiResult<string>.Ok(content);
    }

    public async Task<ApiResult<AuthTokensViewModel>> LoginAsync(LoginViewModel model)
    {
        var baseUrl = _configuration["ApiUrls:Auth"];
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/auth/login", new
        {
            email = model.Email,
            password = model.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return ApiResult<AuthTokensViewModel>.Fail(error);
        }

        var tokens = await response.Content.ReadFromJsonAsync<AuthTokensViewModel>();

        if (tokens == null)
            return ApiResult<AuthTokensViewModel>.Fail("Не удалось получить токены");

        return ApiResult<AuthTokensViewModel>.Ok(tokens);
    }
}
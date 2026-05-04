using WebApp.Models.Auth;
using WebApp.Models.Common;

namespace WebApp.Services;

public interface IAuthApiService
{
    Task<ApiResult<string>> RegisterAsync(RegisterViewModel model);
    Task<ApiResult<AuthTokensViewModel>> LoginAsync(LoginViewModel model);
}
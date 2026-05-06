using WebApp.Models.Auth;
using WebApp.Models.Common;

namespace WebApp.Services;

public interface IAuthApiService
{
    Task<ApiResult<string>> LoginAsync(LoginViewModel model);
    Task<ApiResult<string>> RegisterAsync(RegisterViewModel model);
}

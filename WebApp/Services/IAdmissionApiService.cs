using WebApp.Models.Admission;
using WebApp.Models.Common;

namespace WebApp.Services;

public interface IAdmissionApiService
{
    Task<ApiResult<string>> CreateAsync(CreateAdmissionViewModel model);
    Task<ApiResult<AdmissionViewModel>> GetMyAsync();
}

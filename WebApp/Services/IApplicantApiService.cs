using WebApp.Models.Applicant;
using WebApp.Models.Common;

namespace WebApp.Services;

public interface IApplicantApiService
{
    Task<ApiResult<ProfileViewModel>> GetMyProfileAsync();
    Task<ApiResult<ProfileViewModel>> GetByUserIdAsync(Guid userId);
    Task<ApiResult<string>> CreateAsync(ProfileViewModel model);
    Task<ApiResult<string>> UpdateAsync(ProfileViewModel model);
    Task<ApiResult<string>> UpdateByUserIdAsync(Guid userId, ProfileViewModel model);
}

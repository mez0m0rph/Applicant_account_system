using WebApp.Models.Admission;
using WebApp.Models.Common;
using WebApp.Models.Manager;

namespace WebApp.Services;

public interface IStaffApiService
{
    Task<ApiResult<string>> CreateManagerAsync(CreateManagerViewModel model);
    Task<ApiResult<List<ManagerViewModel>>> GetManagersAsync();
    Task<ApiResult<List<AdmissionViewModel>>> GetAdmissionsAsync();
    Task<ApiResult<string>> AssignManagerAsync(AssignManagerViewModel model);
    Task<ApiResult<string>> ReleaseManagerAsync(Guid admissionId);
    Task<ApiResult<string>> UpdateAdmissionStatusAsync(UpdateAdmissionStatusViewModel model);
}

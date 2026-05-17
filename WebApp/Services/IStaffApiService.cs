using WebApp.Models.Admission;
using WebApp.Models.Common;
using WebApp.Models.Manager;
using WebApp.Models.Staff;

namespace WebApp.Services;

public interface IStaffApiService
{
    Task<ApiResult<string>> CreateManagerAsync(CreateManagerViewModel model);
    Task<ApiResult<List<ManagerViewModel>>> GetManagersAsync();
    Task<ApiResult<ManagerViewModel>> GetManagerByIdAsync(Guid id);
    Task<ApiResult<string>> UpdateManagerAsync(EditManagerViewModel model);
    Task<ApiResult<string>> DeleteManagerAsync(Guid id);

    Task<ApiResult<PagedAdmissionsViewModel>> GetAdmissionsAsync(StaffAdmissionsFilterViewModel filter);
    Task<ApiResult<string>> TakeAdmissionAsync(Guid admissionId);
    Task<ApiResult<string>> ReleaseOwnAdmissionAsync(Guid admissionId);
    Task<ApiResult<string>> AssignManagerAsync(AssignManagerViewModel model);
    Task<ApiResult<string>> ReleaseManagerAsync(Guid admissionId);
    Task<ApiResult<string>> UpdateAdmissionStatusAsync(UpdateAdmissionStatusViewModel model);
}

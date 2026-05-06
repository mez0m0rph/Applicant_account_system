using WebApp.Models.Admission;
using WebApp.Models.Common;

namespace WebApp.Services;

public interface IAdmissionApiService
{
    Task<ApiResult<string>> CreateAsync(CreateAdmissionViewModel model);
    Task<ApiResult<AdmissionViewModel>> GetMyAsync();
    Task<ApiResult<List<AdmissionViewModel>>> GetAllAsync();

    Task<ApiResult<string>> AddProgramAsync(Guid programId, int priority);
    Task<ApiResult<string>> UpdateProgramPriorityAsync(Guid programId, int priority);
    Task<ApiResult<string>> RemoveProgramAsync(Guid programId);
}

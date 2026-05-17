using WebApp.Models.Common;
using WebApp.Models.Program;

namespace WebApp.Services;

public interface IProgramApiService
{
    Task<ApiResult<PagedProgramsViewModel>> GetAllAsync(ProgramsFilterViewModel filter);
}

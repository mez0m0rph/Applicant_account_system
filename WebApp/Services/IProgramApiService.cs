using WebApp.Models.Common;
using WebApp.Models.Program;

namespace WebApp.Services;

public interface IProgramApiService
{
    Task<ApiResult<PagedProgramsViewModel>> GetAllAsync(ProgramsFilterViewModel filter);
    Task<ApiResult<string>> ImportCatalogsAsync();
    Task<ApiResult<ProgramImportStatusViewModel>> GetImportStatusAsync();
}

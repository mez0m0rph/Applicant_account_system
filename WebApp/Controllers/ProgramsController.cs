using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Program;
using WebApp.Services;

namespace WebApp.Controllers;

public class ProgramsController : Controller
{
    private readonly IProgramApiService _programApiService;
    private readonly IAdmissionApiService _admissionApiService;

    public ProgramsController(
        IProgramApiService programApiService,
        IAdmissionApiService admissionApiService)
    {
        _programApiService = programApiService;
        _admissionApiService = admissionApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProgramsFilterViewModel filter)
    {
        if (filter.Page < 1)
            filter.Page = 1;

        if (filter.PageSize < 1)
            filter.PageSize = 10;

        var programsResult = await _programApiService.GetAllAsync(filter);
        var admissionResult = await _admissionApiService.GetMyAsync();

        if (!programsResult.Success)
        {
            TempData["Message"] = programsResult.Error;
            return View(new ProgramsIndexPageViewModel
            {
                Filter = filter
            });
        }

        var selected = new HashSet<Guid>(
            admissionResult.Data?.Programs.Select(x => x.ProgramId) ?? Enumerable.Empty<Guid>());

        var model = new ProgramsIndexPageViewModel
        {
            PagedPrograms = programsResult.Data ?? new PagedProgramsViewModel(),
            Filter = filter,
            SelectedProgramIds = selected
        };

        return View(model);
    }
}

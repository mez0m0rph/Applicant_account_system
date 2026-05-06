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
    public async Task<IActionResult> Index()
    {
        var programsResult = await _programApiService.GetAllAsync();
        var admissionResult = await _admissionApiService.GetMyAsync();

        if (!programsResult.Success)
        {
            TempData["Message"] = programsResult.Error;
            return View(new ProgramsIndexPageViewModel());
        }

        var selected = new HashSet<Guid>(
            admissionResult.Data?.Programs.Select(x => x.ProgramId) ?? Enumerable.Empty<Guid>());

        var model = new ProgramsIndexPageViewModel
        {
            Programs = programsResult.Data ?? new List<ProgramViewModel>(),
            SelectedProgramIds = selected
        };

        return View(model);
    }
}

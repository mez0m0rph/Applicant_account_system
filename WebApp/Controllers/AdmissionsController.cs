using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Services;

namespace WebApp.Controllers;

public class AdmissionsController : Controller
{
    private readonly IAdmissionApiService _admissionApiService;

    public AdmissionsController(IAdmissionApiService admissionApiService)
    {
        _admissionApiService = admissionApiService;
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var result = await _admissionApiService.GetMyAsync();
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateAdmissionViewModel
        {
            Programs = new List<AdmissionProgramItemViewModel>
            {
                new(),
                new(),
                new()
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdmissionViewModel model)
    {
        model.Programs = model.Programs
            .Where(x => x.ProgramId != Guid.Empty && x.Priority > 0)
            .ToList();

        var result = await _admissionApiService.CreateAsync(model);
        TempData["Message"] = result.Success ? "Заявление создано" : result.Error;

        return RedirectToAction("My");
    }
}

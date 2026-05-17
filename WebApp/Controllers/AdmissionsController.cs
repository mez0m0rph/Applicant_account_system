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

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return View(null);
        }

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateAdmissionViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdmissionViewModel model)
    {
        var result = await _admissionApiService.CreateAsync(model);

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return RedirectToAction("My");
        }

        TempData["Message"] = "Заявление создано";
        return RedirectToAction("My");
    }

    [HttpPost]
    public async Task<IActionResult> AddProgram(Guid programId, int priority)
    {
        var result = await _admissionApiService.AddProgramAsync(programId, priority);
        TempData["Message"] = result.Success ? "Программа добавлена" : result.Error;
        return RedirectToAction("Index", "Programs");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProgramPriority(Guid programId, int priority)
    {
        var result = await _admissionApiService.UpdateProgramPriorityAsync(programId, priority);
        TempData["Message"] = result.Success ? "Приоритет обновлен" : result.Error;
        return RedirectToAction("My");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveProgram(Guid programId)
    {
        var result = await _admissionApiService.RemoveProgramAsync(programId);
        TempData["Message"] = result.Success ? "Программа удалена" : result.Error;
        return RedirectToAction("My");
    }
}

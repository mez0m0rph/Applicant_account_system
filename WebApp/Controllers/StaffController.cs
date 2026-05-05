using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Models.Manager;
using WebApp.Services;

namespace WebApp.Controllers;

public class StaffController : Controller
{
    private readonly IStaffApiService _staffApiService;

    public StaffController(IStaffApiService staffApiService)
    {
        _staffApiService = staffApiService;
    }

    [HttpGet]
    public IActionResult CreateManager()
    {
        return View(new CreateManagerViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateManager(CreateManagerViewModel model)
    {
        var result = await _staffApiService.CreateManagerAsync(model);
        TempData["Message"] = result.Success ? "Менеджер создан" : result.Error;
        return RedirectToAction(nameof(Managers));
    }

    [HttpGet]
    public async Task<IActionResult> Managers()
    {
        var result = await _staffApiService.GetManagersAsync();
        return View(result.Data ?? new());
    }

    [HttpGet]
    public async Task<IActionResult> Admissions()
    {
        var result = await _staffApiService.GetAdmissionsAsync();
        return View(result.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> AssignManager(AssignManagerViewModel model)
    {
        var result = await _staffApiService.AssignManagerAsync(model);
        TempData["Message"] = result.Success ? "Менеджер назначен" : result.Error;
        return RedirectToAction(nameof(Admissions));
    }

    [HttpPost]
    public async Task<IActionResult> ReleaseManager(Guid admissionId)
    {
        var result = await _staffApiService.ReleaseManagerAsync(admissionId);
        TempData["Message"] = result.Success ? "Менеджер снят" : result.Error;
        return RedirectToAction(nameof(Admissions));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAdmissionStatus(UpdateAdmissionStatusViewModel model)
    {
        var result = await _staffApiService.UpdateAdmissionStatusAsync(model);
        TempData["Message"] = result.Success ? "Статус обновлен" : result.Error;
        return RedirectToAction(nameof(Admissions));
    }
}

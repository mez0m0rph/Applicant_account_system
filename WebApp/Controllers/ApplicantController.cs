using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Applicant;
using WebApp.Services;

namespace WebApp.Controllers;

public class ApplicantController : Controller
{
    private readonly IApplicantApiService _applicantApiService;

    public ApplicantController(IApplicantApiService applicantApiService)
    {
        _applicantApiService = applicantApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var result = await _applicantApiService.GetMyProfileAsync();

        if (!result.Success || result.Data == null)
            return View(new ProfileViewModel());

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProfileViewModel model)
    {
        var result = await _applicantApiService.CreateAsync(model);
        TempData["Message"] = result.Success ? "Профиль создан" : result.Error;
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProfileViewModel model)
    {
        var result = await _applicantApiService.UpdateAsync(model);
        TempData["Message"] = result.Success ? "Профиль обновлен" : result.Error;
        return RedirectToAction("Profile");
    }
}

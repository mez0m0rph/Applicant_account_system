using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Applicant;
using WebApp.Models.Common;
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
            return View(new ProfileViewModel { HasProfile = false });

        result.Data.HasProfile = true;
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Save(ProfileViewModel model)
    {
        ApiResult<string> result;

        if (model.HasProfile)
            result = await _applicantApiService.UpdateAsync(model);
        else
            result = await _applicantApiService.CreateAsync(model);

        TempData["Message"] = result.Success
            ? (model.HasProfile ? "Профиль обновлен" : "Профиль создан")
            : result.Error;

        return RedirectToAction(nameof(Profile));
    }
}

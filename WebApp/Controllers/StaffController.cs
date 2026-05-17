using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Models.Manager;
using WebApp.Models.Staff;
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
    public async Task<IActionResult> Admissions([FromQuery] StaffAdmissionsFilterViewModel filter)
    {
        if (filter.Page < 1)
            filter.Page = 1;

        if (filter.PageSize < 1)
            filter.PageSize = 10;

        var admissionsResult = await _staffApiService.GetAdmissionsAsync(filter);
        var managersResult = await _staffApiService.GetManagersAsync();

        var model = new StaffAdmissionsPageViewModel
        {
            PagedAdmissions = admissionsResult.Data ?? new PagedAdmissionsViewModel(),
            Filter = filter,
            Managers = managersResult.Data ?? new List<ManagerViewModel>()
        };

        if (!admissionsResult.Success)
            TempData["Message"] = admissionsResult.Error;

        if (!managersResult.Success && TempData["Message"] == null)
            TempData["Message"] = managersResult.Error;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Managers()
    {
        var result = await _staffApiService.GetManagersAsync();

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return View(new List<ManagerViewModel>());
        }

        return View(result.Data ?? new List<ManagerViewModel>());
    }

    [HttpPost]
    public async Task<IActionResult> AssignManager(Guid admissionId, Guid managerUserId)
    {
        var result = await _staffApiService.AssignManagerAsync(new AssignManagerViewModel
        {
            AdmissionId = admissionId,
            ManagerUserId = managerUserId
        });

        TempData["Message"] = result.Success ? "Менеджер назначен" : result.Error;
        return RedirectToAction("Admissions");
    }

    [HttpPost]
    public async Task<IActionResult> ReleaseManager(Guid admissionId)
    {
        var result = await _staffApiService.ReleaseManagerAsync(admissionId);
        TempData["Message"] = result.Success ? "Менеджер снят" : result.Error;
        return RedirectToAction("Admissions");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAdmissionStatus(Guid admissionId, string status)
    {
        var result = await _staffApiService.UpdateAdmissionStatusAsync(new UpdateAdmissionStatusViewModel
        {
            AdmissionId = admissionId,
            Status = status
        });

        TempData["Message"] = result.Success ? "Статус обновлен" : result.Error;
        return RedirectToAction("Admissions");
    }
}

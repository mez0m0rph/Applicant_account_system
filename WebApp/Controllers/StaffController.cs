using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Models.Manager;
using WebApp.Models.Staff;
using WebApp.Services;

namespace WebApp.Controllers;

public class StaffController : Controller
{
    private readonly IStaffApiService _staffApiService;
    private readonly IProgramApiService _programApiService;

    public StaffController(IStaffApiService staffApiService, IProgramApiService programApiService)
    {
        _staffApiService = staffApiService;
        _programApiService = programApiService;
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
    public async Task<IActionResult> EditManager(Guid id)
    {
        var result = await _staffApiService.GetManagerByIdAsync(id);
        if (!result.Success || result.Data == null)
        {
            TempData["Message"] = result.Error;
            return RedirectToAction(nameof(Managers));
        }

        var model = new EditManagerViewModel
        {
            Id = result.Data.Id,
            UserId = result.Data.UserId,
            FullName = result.Data.FullName,
            Email = result.Data.Email,
            Role = result.Data.Role,
            Faculty = result.Data.Faculty
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditManager(EditManagerViewModel model)
    {
        var result = await _staffApiService.UpdateManagerAsync(model);
        TempData["Message"] = result.Success ? "Менеджер обновлен" : result.Error;
        return RedirectToAction(nameof(Managers));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteManager(Guid id)
    {
        var result = await _staffApiService.DeleteManagerAsync(id);
        TempData["Message"] = result.Success ? "Менеджер удален" : result.Error;
        return RedirectToAction(nameof(Managers));
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

    [HttpGet]
    public async Task<IActionResult> Admissions()
    {
        var admissionsResult = await _staffApiService.GetAdmissionsAsync();
        var managersResult = await _staffApiService.GetManagersAsync();
        var programsResult = await _programApiService.GetAllAsync();

        if (!admissionsResult.Success)
            TempData["Message"] = admissionsResult.Error;

        if (!managersResult.Success)
            TempData["Message"] = managersResult.Error;

        if (!programsResult.Success)
            TempData["Message"] = programsResult.Error;

        var admissions = admissionsResult.Data ?? new List<AdmissionViewModel>();
        var managers = managersResult.Data ?? new List<ManagerViewModel>();
        var programs = programsResult.Data ?? new List<WebApp.Models.Program.ProgramViewModel>();

        var managerMap = managers.ToDictionary(x => x.UserId, x => x);
        var programMap = programs.ToDictionary(x => x.Id, x => x);

        foreach (var admission in admissions)
        {
            if (admission.AssignedManagerUserId.HasValue &&
                managerMap.TryGetValue(admission.AssignedManagerUserId.Value, out var manager))
            {
                admission.AssignedManagerName = manager.FullName;
                admission.AssignedManagerEmail = manager.Email;
            }

            foreach (var p in admission.Programs)
            {
                if (programMap.TryGetValue(p.ProgramId, out var program))
                {
                    p.ProgramCode = program.Code;
                    p.ProgramTitle = program.Title;
                }
            }
        }

        var model = new StaffAdmissionsPageViewModel
        {
            Admissions = admissions,
            Managers = managers
        };

        return View(model);
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

using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Services;

namespace WebApp.Controllers;

public class AdmissionsController : Controller
{
    private readonly IAdmissionApiService _admissionApiService;
    private readonly IProgramApiService _programApiService;
    private readonly IStaffApiService _staffApiService;

    public AdmissionsController(
        IAdmissionApiService admissionApiService,
        IProgramApiService programApiService,
        IStaffApiService staffApiService)
    {
        _admissionApiService = admissionApiService;
        _programApiService = programApiService;
        _staffApiService = staffApiService;
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

        var model = result.Data;
        if (model == null)
            return View(null);

        var programsResult = await _programApiService.GetAllAsync();
        if (programsResult.Success && programsResult.Data != null)
        {
            var map = programsResult.Data.ToDictionary(x => x.Id, x => x);
            foreach (var p in model.Programs)
            {
                if (map.TryGetValue(p.ProgramId, out var program))
                {
                    p.ProgramCode = program.Code;
                    p.ProgramTitle = program.Title;
                }
            }
        }

        if (model.AssignedManagerUserId.HasValue)
        {
            var managersResult = await _staffApiService.GetManagersAsync();
            if (managersResult.Success && managersResult.Data != null)
            {
                var manager = managersResult.Data.FirstOrDefault(x => x.UserId == model.AssignedManagerUserId.Value);
                if (manager != null)
                {
                    model.AssignedManagerName = manager.FullName;
                    model.AssignedManagerEmail = manager.Email;
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var result = await _admissionApiService.CreateAsync(new CreateAdmissionViewModel());

        TempData["Message"] = result.Success
            ? "Заявление успешно подано"
            : result.Error;

        return RedirectToAction(nameof(My));
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
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveProgram(Guid programId)
    {
        var result = await _admissionApiService.RemoveProgramAsync(programId);
        TempData["Message"] = result.Success ? "Программа удалена" : result.Error;
        return RedirectToAction(nameof(My));
    }
}

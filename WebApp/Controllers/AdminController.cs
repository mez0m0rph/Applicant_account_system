using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class AdminController : Controller
{
    private readonly IProgramApiService _programApiService;

    public AdminController(IProgramApiService programApiService)
    {
        _programApiService = programApiService;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "Admin";
    }

    private IActionResult ForbiddenRedirect()
    {
        TempData["Message"] = "У вас нет доступа к этому разделу";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Imports()
    {
        if (!IsAdmin())
            return ForbiddenRedirect();

        var statusResult = await _programApiService.GetImportStatusAsync();

        if (!statusResult.Success)
            TempData["Message"] = statusResult.Error;

        return View(statusResult.Data ?? new WebApp.Models.Program.ProgramImportStatusViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> RunImport()
    {
        if (!IsAdmin())
            return ForbiddenRedirect();

        var result = await _programApiService.ImportCatalogsAsync();
        TempData["Message"] = result.Success ? result.Data : result.Error;

        return RedirectToAction("Imports");
    }
}

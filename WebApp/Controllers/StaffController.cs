using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Models.Manager;
using WebApp.Models.Program;
using WebApp.Models.Staff;
using WebApp.Services;

namespace WebApp.Controllers;

public class StaffController : Controller
{
    private readonly IStaffApiService _staffApiService;
    private readonly IProgramApiService _programApiService;
    private readonly IApplicantApiService _applicantApiService;
    private readonly IDocumentApiService _documentApiService;
    private readonly IAdmissionApiService _admissionApiService;

    public StaffController(
        IStaffApiService staffApiService,
        IProgramApiService programApiService,
        IApplicantApiService applicantApiService,
        IDocumentApiService documentApiService,
        IAdmissionApiService admissionApiService)
    {
        _staffApiService = staffApiService;
        _programApiService = programApiService;
        _applicantApiService = applicantApiService;
        _documentApiService = documentApiService;
        _admissionApiService = admissionApiService;
    }

    private string? CurrentRole => HttpContext.Session.GetString("UserRole");

    private bool IsStaff()
    {
        return CurrentRole is "Manager" or "MainManager" or "Admin";
    }

    private bool IsMainManagerOrAdmin()
    {
        return CurrentRole is "MainManager" or "Admin";
    }

    private IActionResult ForbiddenRedirect()
    {
        TempData["Message"] = "У вас нет доступа к этому разделу";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Admissions([FromQuery] StaffAdmissionsFilterViewModel filter)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        if (filter.Page < 1)
            filter.Page = 1;

        if (filter.PageSize < 1)
            filter.PageSize = 10;

        var admissionsResult = await _staffApiService.GetAdmissionsAsync(filter);
        var managersResult = await _staffApiService.GetManagersAsync();
        var programsResult = await _programApiService.GetAllAsync(new ProgramsFilterViewModel
        {
            Page = 1,
            PageSize = 1000
        });

        var model = new StaffAdmissionsPageViewModel
        {
            PagedAdmissions = admissionsResult.Data ?? new PagedAdmissionsViewModel(),
            Filter = filter,
            Managers = managersResult.Data ?? new List<ManagerViewModel>(),
            Programs = programsResult.Data?.Items ?? new List<ProgramViewModel>()
        };

        if (!admissionsResult.Success)
            TempData["Message"] = admissionsResult.Error;

        if (!managersResult.Success && TempData["Message"] == null)
            TempData["Message"] = managersResult.Error;

        if (!programsResult.Success && TempData["Message"] == null)
            TempData["Message"] = programsResult.Error;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid applicantUserId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var profileResult = await _applicantApiService.GetByUserIdAsync(applicantUserId);
        var documentsResult = await _documentApiService.GetByApplicantUserIdAsync(applicantUserId);
        var admissionResult = await _admissionApiService.GetByApplicantUserIdAsync(applicantUserId);

        var model = new StaffApplicantDetailsViewModel
        {
            ApplicantUserId = applicantUserId,
            Profile = profileResult.Success ? profileResult.Data : null,
            Documents = documentsResult.Success ? (documentsResult.Data ?? new List<WebApp.Models.Document.DocumentViewModel>()) : new(),
            Admission = admissionResult.Success ? admissionResult.Data : null
        };

        if (!profileResult.Success && TempData["Message"] == null)
            TempData["Message"] = profileResult.Error;

        if (!documentsResult.Success && TempData["Message"] == null)
            TempData["Message"] = documentsResult.Error;

        if (!admissionResult.Success && TempData["Message"] == null)
            TempData["Message"] = admissionResult.Error;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Managers()
    {
        if (!IsMainManagerOrAdmin())
            return ForbiddenRedirect();

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
        if (!IsStaff())
            return ForbiddenRedirect();

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
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _staffApiService.ReleaseManagerAsync(admissionId);
        TempData["Message"] = result.Success ? "Менеджер снят" : result.Error;
        return RedirectToAction("Admissions");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAdmissionStatus(Guid admissionId, string status)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _staffApiService.UpdateAdmissionStatusAsync(new UpdateAdmissionStatusViewModel
        {
            AdmissionId = admissionId,
            Status = status
        });

        TempData["Message"] = result.Success ? "Статус обновлен" : result.Error;
        return RedirectToAction("Admissions");
    }
}

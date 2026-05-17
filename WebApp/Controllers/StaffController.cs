using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Admission;
using WebApp.Models.Applicant;
using WebApp.Models.Document;
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
        var programsResult = await _programApiService.GetAllAsync(new ProgramsFilterViewModel
        {
            Page = 1,
            PageSize = 1000
        });

        var model = new StaffApplicantDetailsViewModel
        {
            ApplicantUserId = applicantUserId,
            Profile = profileResult.Success ? profileResult.Data : null,
            Documents = documentsResult.Success ? (documentsResult.Data ?? new List<DocumentViewModel>()) : new(),
            Admission = admissionResult.Success ? admissionResult.Data : null,
            AvailablePrograms = programsResult.Success ? (programsResult.Data?.Items ?? new List<ProgramViewModel>()) : new()
        };

        if (!profileResult.Success && TempData["Message"] == null)
            TempData["Message"] = profileResult.Error;

        if (!documentsResult.Success && TempData["Message"] == null)
            TempData["Message"] = documentsResult.Error;

        if (!admissionResult.Success && TempData["Message"] == null)
            TempData["Message"] = admissionResult.Error;

        if (!programsResult.Success && TempData["Message"] == null)
            TempData["Message"] = programsResult.Error;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditApplicant(Guid applicantUserId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _applicantApiService.GetByUserIdAsync(applicantUserId);

        if (!result.Success || result.Data == null)
        {
            TempData["Message"] = result.Error ?? "Профиль абитуриента не найден";
            return RedirectToAction("Details", new { applicantUserId });
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> EditApplicant(ProfileViewModel model)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _applicantApiService.UpdateByUserIdAsync(model.UserId, model);
        TempData["Message"] = result.Success ? "Профиль абитуриента обновлен" : result.Error;

        return RedirectToAction("Details", new { applicantUserId = model.UserId });
    }

    [HttpGet]
    public async Task<IActionResult> EditDocument(Guid applicantUserId, Guid documentId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _documentApiService.GetByApplicantUserIdAsync(applicantUserId);
        var document = result.Data?.FirstOrDefault(x => x.Id == documentId);

        if (document == null)
        {
            TempData["Message"] = "Документ не найден";
            return RedirectToAction("Details", new { applicantUserId });
        }

        ViewBag.ApplicantUserId = applicantUserId;

        return View(new UpdateDocumentViewModel
        {
            Id = document.Id,
            Type = document.Type == "EducationDocument" ? 1 : 0,
            SeriesNumber = document.SeriesNumber,
            IssuedBy = document.IssuedBy,
            BirthPlace = document.BirthPlace,
            IssueDate = document.IssueDate,
            EducationDocumentName = document.EducationDocumentName,
            EducationLevel = document.EducationLevel
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditDocument(Guid applicantUserId, UpdateDocumentViewModel model)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var apiModel = new UpdateDocumentApiModel
        {
            Type = model.Type,
            SeriesNumber = model.SeriesNumber,
            IssuedBy = model.IssuedBy,
            BirthPlace = model.BirthPlace,
            IssueDate = model.IssueDate,
            EducationDocumentName = model.EducationDocumentName,
            EducationLevel = model.EducationLevel
        };

        var result = await _documentApiService.UpdateForApplicantAsync(applicantUserId, model.Id, apiModel);
        TempData["Message"] = result.Success ? "Документ обновлен" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpGet]
    public IActionResult UploadDocument(Guid applicantUserId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        ViewBag.ApplicantUserId = applicantUserId;
        return View(new UploadDocumentViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(Guid applicantUserId, UploadDocumentViewModel model)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        if (model.UploadedFile == null || model.UploadedFile.Length == 0)
        {
            TempData["Message"] = "Файл не выбран";
            return RedirectToAction("UploadDocument", new { applicantUserId });
        }

        await using var memoryStream = new MemoryStream();
        await model.UploadedFile.CopyToAsync(memoryStream);

        var apiModel = new UploadDocumentApiModel
        {
            Type = model.Type,
            FileName = model.UploadedFile.FileName,
            ContentType = model.UploadedFile.ContentType,
            FileContentBase64 = Convert.ToBase64String(memoryStream.ToArray()),
            SeriesNumber = model.SeriesNumber,
            IssuedBy = model.IssuedBy,
            BirthPlace = model.BirthPlace,
            IssueDate = model.IssueDate,
            EducationDocumentName = model.EducationDocumentName,
            EducationLevel = model.EducationLevel
        };

        var result = await _documentApiService.UploadForApplicantAsync(applicantUserId, apiModel);
        TempData["Message"] = result.Success ? "Документ загружен" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDocument(Guid applicantUserId, Guid documentId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _documentApiService.DeleteForApplicantAsync(applicantUserId, documentId);
        TempData["Message"] = result.Success ? "Документ удален" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpGet]
    public IActionResult ReplaceDocumentFile(Guid applicantUserId, Guid documentId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        ViewBag.ApplicantUserId = applicantUserId;
        return View(new ReplaceDocumentFileViewModel
        {
            Id = documentId
        });
    }

    [HttpPost]
    public async Task<IActionResult> ReplaceDocumentFile(Guid applicantUserId, ReplaceDocumentFileViewModel model)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        if (model.UploadedFile == null || model.UploadedFile.Length == 0)
        {
            TempData["Message"] = "Файл не выбран";
            return RedirectToAction("ReplaceDocumentFile", new { applicantUserId, documentId = model.Id });
        }

        await using var memoryStream = new MemoryStream();
        await model.UploadedFile.CopyToAsync(memoryStream);

        var apiModel = new ReplaceDocumentFileApiModel
        {
            FileName = model.UploadedFile.FileName,
            ContentType = model.UploadedFile.ContentType,
            FileContentBase64 = Convert.ToBase64String(memoryStream.ToArray())
        };

        var result = await _documentApiService.ReplaceFileForApplicantAsync(applicantUserId, model.Id, apiModel);
        TempData["Message"] = result.Success ? "Скан документа заменен" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpPost]
    public async Task<IActionResult> AddProgramToAdmission(Guid applicantUserId, Guid admissionId, Guid programId, int priority)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _admissionApiService.AddProgramForStaffAsync(admissionId, programId, priority);
        TempData["Message"] = result.Success ? "Программа добавлена" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProgramPriority(Guid applicantUserId, Guid admissionId, Guid programId, int priority)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _admissionApiService.UpdateProgramPriorityForStaffAsync(admissionId, programId, priority);
        TempData["Message"] = result.Success ? "Приоритет обновлен" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveProgramFromAdmission(Guid applicantUserId, Guid admissionId, Guid programId)
    {
        if (!IsStaff())
            return ForbiddenRedirect();

        var result = await _admissionApiService.RemoveProgramForStaffAsync(admissionId, programId);
        TempData["Message"] = result.Success ? "Программа удалена" : result.Error;

        return RedirectToAction("Details", new { applicantUserId });
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

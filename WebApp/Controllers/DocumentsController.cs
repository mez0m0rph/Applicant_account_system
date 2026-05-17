using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Document;
using WebApp.Services;

namespace WebApp.Controllers;

public class DocumentsController : Controller
{
    private readonly IDocumentApiService _documentApiService;

    public DocumentsController(IDocumentApiService documentApiService)
    {
        _documentApiService = documentApiService;
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var result = await _documentApiService.GetMyAsync();
        return View(result.Data ?? new List<DocumentViewModel>());
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View(new UploadDocumentViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Upload(UploadDocumentViewModel model)
    {
        if (model.UploadedFile == null || model.UploadedFile.Length == 0)
        {
            TempData["Message"] = "Файл не выбран";
            return RedirectToAction("Upload");
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

        var result = await _documentApiService.UploadAsync(apiModel);
        TempData["Message"] = result.Success ? "Документ загружен" : result.Error;

        return RedirectToAction("My");
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid documentId)
    {
        var result = await _documentApiService.DownloadAsync(documentId);

        if (!result.Success || result.Data == null)
        {
            TempData["Message"] = result.Error ?? "Не удалось скачать документ";
            return RedirectToAction("My");
        }

        return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid documentId)
    {
        var result = await _documentApiService.DeleteAsync(documentId);
        TempData["Message"] = result.Success ? "Документ удален" : result.Error;

        return RedirectToAction("My");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid documentId)
    {
        var result = await _documentApiService.GetMyAsync();
        var document = result.Data?.FirstOrDefault(x => x.Id == documentId);

        if (document == null)
        {
            TempData["Message"] = "Документ не найден";
            return RedirectToAction("My");
        }

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
    public async Task<IActionResult> Edit(UpdateDocumentViewModel model)
    {
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

        var result = await _documentApiService.UpdateAsync(model.Id, apiModel);
        TempData["Message"] = result.Success ? "Документ обновлен" : result.Error;

        return RedirectToAction("My");
    }

    [HttpGet]
    public IActionResult ReplaceFile(Guid documentId)
    {
        return View(new ReplaceDocumentFileViewModel
        {
            Id = documentId
        });
    }

    [HttpPost]
    public async Task<IActionResult> ReplaceFile(ReplaceDocumentFileViewModel model)
    {
        if (model.UploadedFile == null || model.UploadedFile.Length == 0)
        {
            TempData["Message"] = "Файл не выбран";
            return RedirectToAction("ReplaceFile", new { documentId = model.Id });
        }

        await using var memoryStream = new MemoryStream();
        await model.UploadedFile.CopyToAsync(memoryStream);

        var apiModel = new ReplaceDocumentFileApiModel
        {
            FileName = model.UploadedFile.FileName,
            ContentType = model.UploadedFile.ContentType,
            FileContentBase64 = Convert.ToBase64String(memoryStream.ToArray())
        };

        var result = await _documentApiService.ReplaceFileAsync(model.Id, apiModel);
        TempData["Message"] = result.Success ? "Скан документа заменен" : result.Error;

        return RedirectToAction("My");
    }
}

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
        return View(result.Data ?? new());
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
            EducationDocumentName = model.EducationDocumentName
        };

        var result = await _documentApiService.UploadAsync(apiModel);
        TempData["Message"] = result.Success ? "Документ загружен" : result.Error;

        return RedirectToAction("My");
    }
}

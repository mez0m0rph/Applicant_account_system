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
        var result = await _documentApiService.UploadAsync(model);
        TempData["Message"] = result.Success ? "Документ загружен" : result.Error;

        return RedirectToAction("My");
    }
}

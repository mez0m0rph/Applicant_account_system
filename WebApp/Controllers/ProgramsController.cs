using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class ProgramsController : Controller
{
    private readonly IProgramApiService _programApiService;

    public ProgramsController(IProgramApiService programApiService)
    {
        _programApiService = programApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _programApiService.GetAllAsync();
        return View(result.Data ?? new());
    }
}

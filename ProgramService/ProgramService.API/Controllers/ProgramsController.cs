using Microsoft.AspNetCore.Mvc;
using ProgramService.Application.Interfaces;

namespace ProgramService.API.Controllers;

[ApiController]
[Route("programs")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramRepository _repository;
    private readonly IProgramImportService _importService;

    public ProgramsController(IProgramRepository repository, IProgramImportService importService)
    {
        _repository = repository;
        _importService = importService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        var count = await _importService.ImportAsync();
        return Ok(new { imported = count });
    }
}

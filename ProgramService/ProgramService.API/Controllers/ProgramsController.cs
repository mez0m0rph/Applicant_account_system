using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramService.Application.DTOs;
using ProgramService.Application.Interfaces;

namespace ProgramService.API.Controllers;

[ApiController]
[Route("programs")]
[Authorize]
public class ProgramsController : ControllerBase
{
    private readonly IProgramRepository _repository;
    private readonly IProgramImportService _importService;

    private static readonly object ImportLock = new();

    private static ProgramImportStatusDto LastImportStatus = new()
    {
        Status = "NeverStarted",
        Message = "Импорт справочников еще не запускался"
    };

    public ProgramsController(
        IProgramRepository repository,
        IProgramImportService importService)
    {
        _repository = repository;
        _importService = importService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? faculty,
        [FromQuery] string? educationLevel,
        [FromQuery] string? educationForm,
        [FromQuery] string? language,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProgramsQuery
        {
            Search = search,
            Faculty = faculty,
            EducationLevel = educationLevel,
            EducationForm = educationForm,
            Language = language,
            Page = page,
            PageSize = pageSize
        };

        var result = await _repository.GetPagedAsync(query);

        return Ok(new
        {
            items = result.Items.Select(x => new
            {
                x.Id,
                x.ExternalId,
                x.Code,
                x.Title,
                x.Description,
                x.BudgetPlaces,
                x.PaidPlaces,
                x.Faculty,
                x.EducationLevel,
                x.EducationForm,
                x.Language,
                x.Duration,
                x.Degree
            }).ToList(),
            page,
            pageSize,
            totalCount = result.TotalCount,
            totalPages = result.TotalCount == 0
                ? 0
                : (int)Math.Ceiling(result.TotalCount / (double)pageSize)
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(new
        {
            result.Id,
            result.ExternalId,
            result.Code,
            result.Title,
            result.Description,
            result.BudgetPlaces,
            result.PaidPlaces,
            result.Faculty,
            result.EducationLevel,
            result.EducationForm,
            result.Language,
            result.Duration,
            result.Degree
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("import")]
    public Task<IActionResult> ImportPrograms()
    {
        return RunCatalogImportAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("~/catalog/import")]
    public Task<IActionResult> ImportCatalogs()
    {
        return RunCatalogImportAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("import/status")]
    public IActionResult GetProgramImportStatus()
    {
        return GetCatalogImportStatusResult();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("~/catalog/import/status")]
    public IActionResult GetCatalogImportStatus()
    {
        return GetCatalogImportStatusResult();
    }

    private async Task<IActionResult> RunCatalogImportAsync()
    {
        lock (ImportLock)
        {
            if (LastImportStatus.Status == "Running")
                return BadRequest(new { error = "Импорт справочников уже выполняется" });

            LastImportStatus = new ProgramImportStatusDto
            {
                Status = "Running",
                StartedAt = DateTime.UtcNow,
                Message = "Импорт справочников выполняется"
            };
        }

        try
        {
            var count = await _importService.ImportAsync();

            lock (ImportLock)
            {
                LastImportStatus = new ProgramImportStatusDto
                {
                    Status = "Completed",
                    StartedAt = LastImportStatus.StartedAt,
                    FinishedAt = DateTime.UtcNow,
                    ImportedCount = count,
                    Message = "Импорт справочников завершен успешно. Обновлен справочник программ и связанные данные факультетов и уровней образования."
                };
            }

            return Ok(new
            {
                imported = count,
                importedPrograms = count,
                message = $"Импорт справочников завершен. Импортировано программ: {count}"
            });
        }
        catch (Exception ex)
        {
            lock (ImportLock)
            {
                LastImportStatus = new ProgramImportStatusDto
                {
                    Status = "Failed",
                    StartedAt = LastImportStatus.StartedAt,
                    FinishedAt = DateTime.UtcNow,
                    ImportedCount = 0,
                    Message = ex.Message
                };
            }

            return BadRequest(new { error = ex.Message });
        }
    }

    private IActionResult GetCatalogImportStatusResult()
    {
        lock (ImportLock)
        {
            return Ok(LastImportStatus);
        }
    }
}
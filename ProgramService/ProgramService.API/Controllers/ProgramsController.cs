using Microsoft.AspNetCore.Mvc;
using ProgramService.Application.DTOs;
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

        var (items, totalCount) = await _repository.GetPagedAsync(query);

        var response = new PagedProgramsResponse
        {
            Items = items.Select(x => new ProgramDto
            {
                Id = x.Id,
                ExternalId = x.ExternalId,
                Code = x.Code,
                Title = x.Title,
                Description = x.Description,
                BudgetPlaces = x.BudgetPlaces,
                PaidPlaces = x.PaidPlaces,
                Faculty = x.Faculty,
                EducationLevel = x.EducationLevel,
                EducationForm = x.EducationForm,
                Language = x.Language,
                Duration = x.Duration,
                Degree = x.Degree
            }).ToList(),
            Page = page < 1 ? 1 : page,
            PageSize = pageSize < 1 ? 10 : pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)(pageSize < 1 ? 10 : pageSize))
        };

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        var count = await _importService.ImportAsync();
        return Ok(new { imported = count });
    }
}

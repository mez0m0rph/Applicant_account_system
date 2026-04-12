using Microsoft.AspNetCore.Mvc;
using ProgramService.Application.Interfaces;

namespace ProgramService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IStudyProgramService _service;
    public ProgramsController(IStudyProgramService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var programs = await _service.GetAllAsync();
        return Ok(programs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var program = await _service.GetByIdAsync(id);

        if (program == null) 
            return NotFound();

        return Ok(program);
    }


    [HttpGet("faculty/{faculty}")]
    public async Task<IActionResult> GetByFaculty(string faculty)
    {
        var programs = await _service.GetByFacultyAsync(faculty);
        return Ok(programs);
    }

    [HttpGet("degree/{degree}")]
    public async Task<IActionResult> GetByDegree(string degree)
    {
        var programs = await _service.GetByDegreeAsync(degree);
        return Ok(programs);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? faculty, 
        [FromQuery]string? degree,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var programs = await _service.SearchAsync(faculty, degree, page, pageSize);
        return Ok(programs);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        await _service.SyncProgramsAsync();
        return Ok("Sync completed");
    }
}
using Microsoft.AspNetCore.Mvc;
using ProgramService.Services;

namespace ProgramService.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetById(Guid id)
    {
        var program = await _service.GetByIdAsync(id);

        if (program == null)
        {
            return NotFound();
        }

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
        return Ok(degree);
    }

}
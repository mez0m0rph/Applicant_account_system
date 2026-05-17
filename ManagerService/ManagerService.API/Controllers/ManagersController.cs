using ManagerService.Application.DTOs;
using ManagerService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerService.API.Controllers;

[ApiController]
[Route("managers")]
[Authorize]
public class ManagersController : ControllerBase
{
    private readonly IManagerService _service;

    public ManagersController(IManagerService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "MainManager,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateManagerRequest request)
    {
        var id = await _service.CreateAsync(request);
        return Ok(new { id });
    }

    [Authorize(Roles = "MainManager,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateManagerRequest request)
    {
        await _service.UpdateAsync(id, request);
        return Ok("Менеджер обновлен");
    }

    [Authorize(Roles = "MainManager,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok("Менеджер удален");
    }
}

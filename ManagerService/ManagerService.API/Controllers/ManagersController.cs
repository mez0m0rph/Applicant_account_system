using System.Security.Claims;
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
    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateManagerRequest request)
    {
        await _service.CreateAsync(request);
        return Ok("Менеджер создан");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var manager = await _service.GetByIdAsync(id);
        return Ok(manager);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var managers = await _service.GetAllAsync();
        return  Ok(managers);
    }
}
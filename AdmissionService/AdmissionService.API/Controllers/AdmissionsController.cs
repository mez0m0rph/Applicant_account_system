using System.Security.Claims;
using AdmissionService.Application.DTOs;
using AdmissionService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionService.API;

[ApiController]
[Route("/[controller]")]
[Authorize]
public class AdmissionsController : ControllerBase
{
    private readonly IAdmissionService _service;
    public AdmissionsController(IAdmissionService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim)) 
            throw new Exception("Пользователь не зарегистрирован");

        return Guid.Parse(userIdClaim);  
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmission([FromBody] CreateAdmissionRequest request)
    {
        var userId = GetUserId();
        await _service.CreateAdmissionAsync(userId, request);
        return Ok("Заявка создана");
    }

    
    [HttpGet("me")]
    public async Task<IActionResult> GetAdmission()
    {
        var userId = GetUserId();
        var admission = await _service.GetMyAdmissionAsync(userId);

        return Ok(admission);
    }

}
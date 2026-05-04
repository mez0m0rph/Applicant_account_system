using AdmissionService.Application.DTOs;
using AdmissionService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdmissionService.API.Controllers;

[ApiController]
[Route("admissions")]
[Authorize]
public class AdmissionsController : ControllerBase
{
    private readonly IAdmissionService _service;

    public AdmissionsController(IAdmissionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdmissionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email не найден в токене");

        await _service.CreateAdmissionAsync(applicantUserId, email, request);
        return Ok("Заявление создано");
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyAdmissionAsync(applicantUserId);
        return Ok(result);
    }
}

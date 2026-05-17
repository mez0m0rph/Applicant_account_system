using ApplicantService.Application.DTOs;
using ApplicantService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicantService.API.Controllers;

[ApiController]
[Route("applicant")]
[Authorize]
public class ApplicantController : ControllerBase
{
    private readonly IApplicantService _service;

    public ApplicantController(IApplicantService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicantRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Некорректный user id");

        await _service.CreateAsync(userId, request);
        return Ok("Профиль создан");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyAsync(userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _service.GetMyAsync(userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> Update([FromBody] UpdateApplicantRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Некорректный user id");

        await _service.UpdateAsync(userId, request);
        return Ok("Профиль обновлен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateByUserId(Guid userId, [FromBody] UpdateApplicantRequest request)
    {
        await _service.UpdateAsync(userId, request);
        return Ok("Профиль абитуриента обновлен");
    }
}

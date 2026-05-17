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
    private readonly IAdmissionCatalogClient _admissionCatalogClient;

    public ApplicantController(IApplicantService service, IAdmissionCatalogClient admissionCatalogClient)
    {
        _service = service;
        _admissionCatalogClient = admissionCatalogClient;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private string? GetCurrentRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value
               ?? User.FindFirst("role")?.Value;
    }

    private async Task<bool> CanEditApplicantAsync(Guid applicantUserId)
    {
        var role = GetCurrentRole();
        if (role is "MainManager" or "Admin")
            return true;

        if (role == "Manager")
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return false;

            var admission = await _admissionCatalogClient.GetByApplicantUserIdAsync(applicantUserId);
            return admission != null && admission.AssignedManagerUserId == currentUserId.Value;
        }

        return false;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicantRequest request)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.CreateAsync(userId.Value, request);
        return Ok("Профиль создан");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyAsync(userId.Value);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [AllowAnonymous]
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
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.UpdateAsync(userId.Value, request);
        return Ok("Профиль обновлен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateByUserId(Guid userId, [FromBody] UpdateApplicantRequest request)
    {
        if (!await CanEditApplicantAsync(userId))
            return Forbid();

        await _service.UpdateAsync(userId, request);
        return Ok("Профиль абитуриента обновлен");
    }
}

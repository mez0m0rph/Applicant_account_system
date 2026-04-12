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
    private readonly IApplicantService _applicantService;

    public ApplicantController(IApplicantService applicantService)
    {
        _applicantService = applicantService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new Exception("Пользователь не авторизован");

        return Guid.Parse(userIdClaim);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var userId = GetUserId();
        await _applicantService.CreateAsync(userId, request);
        return Ok("Профиль создан");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        var profile = _applicantService.GetMyProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();
        await _applicantService.UpdateAsync(userId, request);
        return Ok("Профиль обновлен");
    }
    
}
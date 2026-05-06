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
    public async Task<IActionResult> Create()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email не найден в токене");

        await _service.CreateAsync(applicantUserId, email);
        return Ok("Заявление создано");
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyAsync(applicantUserId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost("my/programs")]
    public async Task<IActionResult> AddProgram([FromBody] AddProgramToAdmissionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.AddProgramAsync(applicantUserId, request.ProgramId, request.Priority);
        return Ok("Программа добавлена в заявление");
    }

    [HttpPut("my/programs/{programId:guid}/priority")]
    public async Task<IActionResult> UpdatePriority(Guid programId, [FromBody] UpdateAdmissionProgramPriorityRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.UpdateProgramPriorityAsync(applicantUserId, programId, request.Priority);
        return Ok("Приоритет обновлен");
    }

    [HttpDelete("my/programs/{programId:guid}")]
    public async Task<IActionResult> RemoveProgram(Guid programId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.RemoveProgramAsync(applicantUserId, programId);
        return Ok("Программа удалена из заявления");
    }

    [HttpPost("{id:guid}/assign-manager")]
    public async Task<IActionResult> AssignManager(Guid id, [FromBody] AssignManagerRequest request)
    {
        await _service.AssignManagerAsync(id, request.ManagerUserId, request.ManagerEmail);
        return Ok("Менеджер назначен");
    }

    [HttpPost("{id:guid}/release-manager")]
    public async Task<IActionResult> ReleaseManager(Guid id)
    {
        await _service.ReleaseManagerAsync(id);
        return Ok("Менеджер снят");
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAdmissionStatusRequest request)
    {
        await _service.UpdateStatusAsync(id, request.Status);
        return Ok("Статус обновлен");
    }
}

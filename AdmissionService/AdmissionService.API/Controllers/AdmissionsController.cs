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

    [Authorize(Roles = "Applicant")]
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var applicantUserId = GetCurrentUserId();
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email не найден в токене");

        await _service.CreateAsync(applicantUserId.Value, email);
        return Ok("Заявление создано");
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyAsync(applicantUserId.Value);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? programId,
        [FromQuery] List<string>? faculties,
        [FromQuery] string? status,
        [FromQuery] bool onlyUnassigned = false,
        [FromQuery] bool onlyMine = false,
        [FromQuery] Guid? assignedManagerUserId = null,
        [FromQuery] string sortBy = "updatedAt",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAdmissionsQuery
        {
            Search = search,
            ProgramId = programId,
            Faculties = faculties ?? new List<string>(),
            Status = status,
            OnlyUnassigned = onlyUnassigned,
            OnlyMine = onlyMine,
            AssignedManagerUserId = assignedManagerUserId,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };

        return Ok(await _service.GetPagedAsync(query, GetCurrentUserId(), GetCurrentRole()));
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet("applicant/{applicantUserId:guid}")]
    public async Task<IActionResult> GetByApplicantUserId(Guid applicantUserId)
    {
        var result = await _service.GetMyAsync(applicantUserId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Applicant")]
    [HttpPost("my/programs")]
    public async Task<IActionResult> AddProgram([FromBody] AddProgramToAdmissionRequest request)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.AddProgramAsync(applicantUserId.Value, request.ProgramId, request.Priority);
        return Ok("Программа добавлена в заявление");
    }

    [Authorize(Roles = "Applicant")]
    [HttpPut("my/programs/{programId:guid}/priority")]
    public async Task<IActionResult> UpdatePriority(Guid programId, [FromBody] UpdateAdmissionProgramPriorityRequest request)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.UpdateProgramPriorityAsync(applicantUserId.Value, programId, request.Priority);
        return Ok("Приоритет обновлен");
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("my/programs/{programId:guid}")]
    public async Task<IActionResult> RemoveProgram(Guid programId)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.RemoveProgramAsync(applicantUserId.Value, programId);
        return Ok("Программа удалена из заявления");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPost("{id:guid}/programs")]
    public async Task<IActionResult> AddProgramForStaff(Guid id, [FromBody] AddProgramToAdmissionRequest request)
    {
        await _service.AddProgramForStaffAsync(id, request.ProgramId, request.Priority, GetCurrentUserId(), GetCurrentRole());
        return Ok("Программа добавлена в заявление");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("{id:guid}/programs/{programId:guid}/priority")]
    public async Task<IActionResult> UpdatePriorityForStaff(Guid id, Guid programId, [FromBody] UpdateAdmissionProgramPriorityRequest request)
    {
        await _service.UpdateProgramPriorityForStaffAsync(id, programId, request.Priority, GetCurrentUserId(), GetCurrentRole());
        return Ok("Приоритет обновлен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpDelete("{id:guid}/programs/{programId:guid}")]
    public async Task<IActionResult> RemoveProgramForStaff(Guid id, Guid programId)
    {
        await _service.RemoveProgramForStaffAsync(id, programId, GetCurrentUserId(), GetCurrentRole());
        return Ok("Программа удалена из заявления");
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{id:guid}/take")]
    public async Task<IActionResult> Take(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.TakeAdmissionAsync(id, currentUserId.Value);
        return Ok("Поступление взято в работу");
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{id:guid}/release-own")]
    public async Task<IActionResult> ReleaseOwn(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.ReleaseOwnAdmissionAsync(id, currentUserId.Value);
        return Ok("Поступление возвращено в общий пул");
    }

    [Authorize(Roles = "MainManager,Admin")]
    [HttpPost("{id:guid}/assign-manager")]
    public async Task<IActionResult> AssignManager(Guid id, [FromBody] AssignManagerRequest request)
    {
        await _service.AssignManagerAsync(id, request.ManagerUserId, request.ManagerEmail, GetCurrentUserId(), GetCurrentRole());
        return Ok("Менеджер назначен");
    }

    [Authorize(Roles = "MainManager,Admin")]
    [HttpPost("{id:guid}/release-manager")]
    public async Task<IActionResult> ReleaseManager(Guid id)
    {
        await _service.ReleaseManagerAsync(id, GetCurrentUserId(), GetCurrentRole());
        return Ok("Менеджер снят");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAdmissionStatusRequest request)
    {
        await _service.UpdateStatusAsync(id, request.Status, GetCurrentUserId(), GetCurrentRole());
        return Ok("Статус обновлен");
    }
}

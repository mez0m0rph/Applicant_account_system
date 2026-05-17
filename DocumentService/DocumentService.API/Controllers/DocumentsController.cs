using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocumentService.API.Controllers;

[ApiController]
[Route("documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _service;
    private readonly IAdmissionCatalogClient _admissionCatalogClient;

    public DocumentsController(IDocumentService service, IAdmissionCatalogClient admissionCatalogClient)
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
    public async Task<IActionResult> Upload([FromBody] UploadDocumentRequest request)
    {
        var applicantUserId = GetCurrentUserId();
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email не найден в токене");

        await _service.UploadAsync(applicantUserId.Value, email, request);
        return Ok("Документ загружен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPost("applicant/{applicantUserId:guid}")]
    public async Task<IActionResult> UploadForApplicant(Guid applicantUserId, [FromBody] UploadDocumentRequest request)
    {
        if (!await CanEditApplicantAsync(applicantUserId))
            return Forbid();

        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "staff@local";
        await _service.UploadAsync(applicantUserId, email, request);
        return Ok("Документ загружен");
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyDocumentsAsync(applicantUserId.Value);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("applicant/{applicantUserId:guid}")]
    public async Task<IActionResult> GetByApplicant(Guid applicantUserId)
    {
        var result = await _service.GetMyDocumentsAsync(applicantUserId);
        return Ok(result);
    }

    [HttpGet("my/{documentId:guid}/download")]
    public async Task<IActionResult> Download(Guid documentId)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        var file = await _service.DownloadAsync(applicantUserId.Value, documentId);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpGet("applicant/{applicantUserId:guid}/{documentId:guid}/download")]
    public async Task<IActionResult> DownloadForApplicant(Guid applicantUserId, Guid documentId)
    {
        var file = await _service.DownloadAsync(applicantUserId, documentId);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpDelete("my/{documentId:guid}")]
    public async Task<IActionResult> Delete(Guid documentId)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.DeleteAsync(applicantUserId.Value, documentId);
        return Ok("Документ удален");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpDelete("applicant/{applicantUserId:guid}/{documentId:guid}")]
    public async Task<IActionResult> DeleteForApplicant(Guid applicantUserId, Guid documentId)
    {
        if (!await CanEditApplicantAsync(applicantUserId))
            return Forbid();

        await _service.DeleteAsync(applicantUserId, documentId);
        return Ok("Документ удален");
    }

    [HttpPut("my/{documentId:guid}")]
    public async Task<IActionResult> Update(Guid documentId, [FromBody] UpdateDocumentRequest request)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.UpdateAsync(applicantUserId.Value, documentId, request);
        return Ok("Документ обновлен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("applicant/{applicantUserId:guid}/{documentId:guid}")]
    public async Task<IActionResult> UpdateForApplicant(Guid applicantUserId, Guid documentId, [FromBody] UpdateDocumentRequest request)
    {
        if (!await CanEditApplicantAsync(applicantUserId))
            return Forbid();

        await _service.UpdateAsync(applicantUserId, documentId, request);
        return Ok("Документ обновлен");
    }

    [HttpPut("my/{documentId:guid}/file")]
    public async Task<IActionResult> ReplaceFile(Guid documentId, [FromBody] ReplaceDocumentFileRequest request)
    {
        var applicantUserId = GetCurrentUserId();

        if (!applicantUserId.HasValue)
            return Unauthorized("Некорректный user id");

        await _service.ReplaceFileAsync(applicantUserId.Value, documentId, request);
        return Ok("Файл документа заменен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("applicant/{applicantUserId:guid}/{documentId:guid}/file")]
    public async Task<IActionResult> ReplaceFileForApplicant(Guid applicantUserId, Guid documentId, [FromBody] ReplaceDocumentFileRequest request)
    {
        if (!await CanEditApplicantAsync(applicantUserId))
            return Forbid();

        await _service.ReplaceFileAsync(applicantUserId, documentId, request);
        return Ok("Файл документа заменен");
    }
}

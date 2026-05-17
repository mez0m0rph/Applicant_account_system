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

    public DocumentsController(IDocumentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] UploadDocumentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized("Email не найден в токене");

        await _service.UploadAsync(applicantUserId, email, request);
        return Ok("Документ загружен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPost("applicant/{applicantUserId:guid}")]
    public async Task<IActionResult> UploadForApplicant(Guid applicantUserId, [FromBody] UploadDocumentRequest request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "staff@local";

        await _service.UploadAsync(applicantUserId, email, request);
        return Ok("Документ загружен");
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyDocumentsAsync(applicantUserId);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        var file = await _service.DownloadAsync(applicantUserId, documentId);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.DeleteAsync(applicantUserId, documentId);
        return Ok("Документ удален");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpDelete("applicant/{applicantUserId:guid}/{documentId:guid}")]
    public async Task<IActionResult> DeleteForApplicant(Guid applicantUserId, Guid documentId)
    {
        await _service.DeleteAsync(applicantUserId, documentId);
        return Ok("Документ удален");
    }

    [HttpPut("my/{documentId:guid}")]
    public async Task<IActionResult> Update(Guid documentId, [FromBody] UpdateDocumentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.UpdateAsync(applicantUserId, documentId, request);
        return Ok("Документ обновлен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("applicant/{applicantUserId:guid}/{documentId:guid}")]
    public async Task<IActionResult> UpdateForApplicant(Guid applicantUserId, Guid documentId, [FromBody] UpdateDocumentRequest request)
    {
        await _service.UpdateAsync(applicantUserId, documentId, request);
        return Ok("Документ обновлен");
    }

    [HttpPut("my/{documentId:guid}/file")]
    public async Task<IActionResult> ReplaceFile(Guid documentId, [FromBody] ReplaceDocumentFileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        await _service.ReplaceFileAsync(applicantUserId, documentId, request);
        return Ok("Файл документа заменен");
    }

    [Authorize(Roles = "Manager,MainManager,Admin")]
    [HttpPut("applicant/{applicantUserId:guid}/{documentId:guid}/file")]
    public async Task<IActionResult> ReplaceFileForApplicant(Guid applicantUserId, Guid documentId, [FromBody] ReplaceDocumentFileRequest request)
    {
        await _service.ReplaceFileAsync(applicantUserId, documentId, request);
        return Ok("Файл документа заменен");
    }
}

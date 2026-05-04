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

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var applicantUserId))
            return Unauthorized("Некорректный user id");

        var result = await _service.GetMyDocumentsAsync(applicantUserId);
        return Ok(result);
    }
}

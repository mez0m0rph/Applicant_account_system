using System.Security.Claims;
using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new Exception("Пользователь не авторизован");

        return Guid.Parse(userIdClaim);
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] UploadDocumentRequest request)
    {
        var userId = GetUserId();
        await _service.UploadAsync(userId, request);
        return Ok("Документ загружен");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyDocuments()
    {
        var userId = GetUserId();
        var documents = await _service.GetMyDocumentsAsync(userId);
        return Ok(documents);
    }
}
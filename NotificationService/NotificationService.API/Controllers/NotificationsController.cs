using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
    {
        await _service.CreateAsync(request);
        return Ok("Уведомление создано");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _service.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var notification = await _service.GetByIdAsync(id);

        if (notification == null)
            return NotFound();

        return Ok(notification);
    }

    [HttpPost("{id:guid}/mark-sent")]
    public async Task<IActionResult> MarkAsSent(Guid id)
    {
        await _service.MarkAsSentAsync(id);
        return Ok("Уведомление помечено как отправленное");
    }
}
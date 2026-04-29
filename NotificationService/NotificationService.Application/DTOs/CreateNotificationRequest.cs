using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
}
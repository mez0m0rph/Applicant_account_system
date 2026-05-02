namespace Shared.Contracts.Events;

public class NotificationRequestedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
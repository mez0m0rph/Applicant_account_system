using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetAllAsync();
    Task<NotificationResponse?> GetByIdAsync(Guid id);
    Task CreateAsync(CreateNotificationRequest request);
    Task MarkAsSentAsync(Guid id);
}
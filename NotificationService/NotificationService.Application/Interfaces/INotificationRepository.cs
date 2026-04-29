using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync();
    Task<Notification?> GetByIdAsync(Guid id);
    Task CreateAsync(Notification notification);
    Task UpdateAsync(Notification notification);
}
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class NotificationServiceImpl : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationServiceImpl(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotificationResponse>> GetAllAsync()
    {
        var notifications = await _repository.GetAllAsync();

        return notifications.Select(notification => new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Email = notification.Email,
            Subject = notification.Subject,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            IsSent = notification.IsSent,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt
        }).ToList();
    }

    public async Task<NotificationResponse?> GetByIdAsync(Guid id)
    {
        var notification = await _repository.GetByIdAsync(id);

        if (notification == null)
            return null;

        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Email = notification.Email,
            Subject = notification.Subject,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            IsSent = notification.IsSent,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt
        };
    }

    public async Task CreateAsync(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            Type = request.Type,
            IsSent = false,
            CreatedAt = DateTime.UtcNow,
            SentAt = null
        };

        await _repository.CreateAsync(notification);
    }

    public async Task MarkAsSentAsync(Guid id)
    {
        var notification = await _repository.GetByIdAsync(id);

        if (notification == null)
            throw new Exception("Такого уведомления не существует");

        notification.IsSent = true;
        notification.SentAt = DateTime.UtcNow;

        await _repository.UpdateAsync(notification);
    }
}
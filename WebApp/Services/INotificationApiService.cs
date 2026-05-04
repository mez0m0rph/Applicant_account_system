using WebApp.Models.Common;
using WebApp.Models.Notification;

namespace WebApp.Services;

public interface INotificationApiService
{
    Task<ApiResult<List<NotificationViewModel>>> GetAllAsync();
}

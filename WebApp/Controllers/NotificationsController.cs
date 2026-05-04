using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class NotificationsController : Controller
{
    private readonly INotificationApiService _notificationApiService;

    public NotificationsController(INotificationApiService notificationApiService)
    {
        _notificationApiService = notificationApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _notificationApiService.GetAllAsync();
        return View(result.Data ?? new());
    }
}

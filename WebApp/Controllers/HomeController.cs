using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        ViewBag.IsAuthenticated = !string.IsNullOrWhiteSpace(token);
        ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
        return View();
    }
}

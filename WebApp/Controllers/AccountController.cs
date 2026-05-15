using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Auth;
using WebApp.Services;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;

    public AccountController(IAuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new WebApp.Models.Account.ChangePasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(WebApp.Models.Account.ChangePasswordViewModel model)
    {
        var result = await _authApiService.ChangePasswordAsync(model);

        TempData["Message"] = result.Success ? "Пароль успешно изменен" : result.Error;
        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var result = await _authApiService.LoginAsync(model);

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return View(model);
        }

        TempData["Message"] = "Вы успешно вошли в систему";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var result = await _authApiService.RegisterAsync(model);

        if (!result.Success)
        {
            TempData["Message"] = result.Error;
            return View(model);
        }

        TempData["Message"] = "Регистрация успешна. Теперь войдите в систему.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Message"] = "Вы вышли из системы";
        return RedirectToAction(nameof(Login));
    }
}

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

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.LoginAsync(model);

        if (!result.Success || result.Data == null)
        {
            ViewBag.Error = result.Error;
            return View(model);
        }

        HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
        HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);

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
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.RegisterAsync(model);

        if (!result.Success)
        {
            ViewBag.Error = result.Error;
            return View(model);
        }

        return RedirectToAction("Login");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
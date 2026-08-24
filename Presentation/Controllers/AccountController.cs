using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
  public class AccountController : Controller
  {
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
      if (!string.IsNullOrEmpty(HttpContext.Session.GetString("AccessToken")))
      {
        return RedirectToAction("Index", "Dashboard");
      }

      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
      var accessToken = HttpContext.Session.GetString("AccessToken");

      if (string.IsNullOrEmpty(accessToken))
      {
        return RedirectToAction("Login");
      }

      var user = await _authService.GetCurrentUserAsync(accessToken);

      if (user == null)
      {
        HttpContext.Session.Clear();
        TempData["ErrorMessage"] = "Session Anda telah berakhir. Silakan login kembali.";
        return RedirectToAction("Login");
      }

      HttpContext.Session.SetString("FullName", user.FullName);
      HttpContext.Session.SetString("Email", user.Email);
      HttpContext.Session.SetString("Image", user.Image);
      HttpContext.Session.SetString("Role", user.Role);

      return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
      if (!ModelState.IsValid)
      {
        return View(dto);
      }

      var result = await _authService.LoginAsync(dto);

      if (result == null)
      {
        ViewBag.ErrorMessage = "Username atau password salah.";
        return View(dto);
      }

      HttpContext.Session.SetString("AccessToken", result.AccessToken);
      HttpContext.Session.SetString("RefreshToken", result.RefreshToken);
      HttpContext.Session.SetString("Username", result.Username);
      HttpContext.Session.SetString("FullName", result.FullName);
      HttpContext.Session.SetString("Email", result.Email);
      HttpContext.Session.SetString("Image", result.Image);

      var currentUser = await _authService.GetCurrentUserAsync(result.AccessToken);
      HttpContext.Session.SetString("Role", currentUser?.Role ?? "user");

      return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      return RedirectToAction("Login");
    }
  }
}
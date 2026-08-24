using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Controllers
{
  public class BaseController : Controller
  {
    protected IAuthService AuthService { get; }

    public BaseController(IAuthService authService)
    {
      AuthService = authService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var accessToken = HttpContext.Session.GetString("AccessToken");

      if (string.IsNullOrEmpty(accessToken))
      {
        context.Result = RedirectToAction("Login", "Account");
        return;
      }

      ViewBag.FullName = HttpContext.Session.GetString("FullName");
      ViewBag.Username = HttpContext.Session.GetString("Username");
      ViewBag.Email = HttpContext.Session.GetString("Email");
      ViewBag.Image = HttpContext.Session.GetString("Image");

      base.OnActionExecuting(context);
    }

    protected async Task<string?> GetValidAccessTokenAsync()
    {
      var accessToken = HttpContext.Session.GetString("AccessToken");
      var refreshToken = HttpContext.Session.GetString("RefreshToken");

      if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
      {
        return null;
      }

      var user = await AuthService.GetCurrentUserAsync(accessToken);

      if (user != null)
      {
        return accessToken;
      }

      var refreshResult = await AuthService.RefreshTokenAsync(refreshToken);

      if (refreshResult == null)
      {
        HttpContext.Session.Clear();
        return null;
      }

      HttpContext.Session.SetString("AccessToken", refreshResult.AccessToken);
      HttpContext.Session.SetString("RefreshToken", refreshResult.RefreshToken);

      return refreshResult.AccessToken;
    }
  }
}
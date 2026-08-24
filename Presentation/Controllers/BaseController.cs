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

    protected bool IsAdmin => HttpContext.Session.GetString("Role") == "admin";

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
      ViewBag.IsAdmin = IsAdmin;

      base.OnActionExecuting(context);
    }

    protected async Task<string?> GetValidAccessTokenAsync()
    {
      var accessToken = HttpContext.Session.GetString("AccessToken");

      if (string.IsNullOrEmpty(accessToken))
      {
        return null;
      }

      return accessToken;
    }
  }
}
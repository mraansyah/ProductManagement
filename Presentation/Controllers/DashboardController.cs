using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
  public class DashboardController : BaseController
  {
    private readonly IProductService _productService;

    public DashboardController(IAuthService authService, IProductService productService)
        : base(authService)
    {
      _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
      var token = await GetValidAccessTokenAsync();

      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      var summary = await _productService.GetDashboardSummaryAsync();

      return View(summary);
    }
  }
}
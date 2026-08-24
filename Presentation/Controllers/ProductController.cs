using System.Net;
using Business.Services;
using Business.DTOs;
using Data.Models;
using Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
  public class ProductController : BaseController
  {
    private readonly IProductService _productService;

    public ProductController(IAuthService authService, IProductService productService)
        : base(authService)
    {
      _productService = productService;
    }

    private string GetErrorMessage(HttpStatusCode statusCode)
    {
      return statusCode switch
      {
        HttpStatusCode.BadRequest => "Data yang dikirim tidak valid.",
        HttpStatusCode.Unauthorized => "Session Anda telah berakhir. Silakan login kembali.",
        HttpStatusCode.Forbidden => "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.",
        HttpStatusCode.NotFound => "Product tidak ditemukan.",
        HttpStatusCode.InternalServerError => "Terjadi kesalahan pada server. Silakan coba kembali.",
        _ => "Data product gagal dimuat. Silakan coba kembali."
      };
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      const int pageSize = 10;

      if (page < 1) page = 1;

      var apiResult = await _productService.GetProductsAsync(page, pageSize, search);

      if (!apiResult.IsSuccess || apiResult.Data == null)
      {
        ViewBag.ErrorMessage = GetErrorMessage(apiResult.StatusCode);
        return View(new ProductListViewModel());
      }

      var viewModel = new ProductListViewModel
      {
        Products = apiResult.Data.Products,
        CurrentPage = page,
        PageSize = pageSize,
        TotalItems = apiResult.Data.Total,
        SearchQuery = search
      };

      return View(viewModel);
    }

    public async Task<IActionResult> Detail(int id)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      var apiResult = await _productService.GetProductByIdAsync(id);

      if (apiResult.IsSuccess && apiResult.Data != null)
      {
        return View(apiResult.Data);
      }

      if (apiResult.StatusCode == HttpStatusCode.NotFound)
      {
        var fallback = new Product
        {
          Id = id,
          Title = $"Product #{id}",
          Category = "General",
          Description = "Detail product",
          Price = 0,
          Stock = 0
        };
        return View(fallback);
      }

      TempData["ErrorMessage"] = GetErrorMessage(apiResult.StatusCode);
      return RedirectToAction("Index");
    }
    public async Task<IActionResult> Create()
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      if (!IsAdmin)
      {
        TempData["ErrorMessage"] = "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.";
        return RedirectToAction("Index");
      }

      await PopulateCategoriesAndBrandsAsync();
      return View(new ProductFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormDto dto)
    {
      if (!IsAdmin)
      {
        TempData["ErrorMessage"] = "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.";
        return RedirectToAction("Index");
      }

      if (!ModelState.IsValid)
      {
        await PopulateCategoriesAndBrandsAsync();
        return View(dto);
      }

      var product = new Product
      {
        Title = dto.Title,
        Description = dto.Description,
        Category = dto.Category,
        Price = dto.Price,
        Stock = dto.Stock,
        Brand = dto.Brand
      };

      var apiResult = await _productService.CreateProductAsync(product);

      if (!apiResult.IsSuccess || apiResult.Data == null)
      {
        ViewBag.ErrorMessage = GetErrorMessage(apiResult.StatusCode);
        await PopulateCategoriesAndBrandsAsync();
        return View(dto);
      }

      TempData["SuccessMessage"] = $"Product '{apiResult.Data.Title}' berhasil ditambahkan.";
      TempData["NewProduct"] = System.Text.Json.JsonSerializer.Serialize(apiResult.Data);
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      if (!IsAdmin)
      {
        TempData["ErrorMessage"] = "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.";
        return RedirectToAction("Index");
      }

      var apiResult = await _productService.GetProductByIdAsync(id);

      ProductFormDto dto;
      if (apiResult.IsSuccess && apiResult.Data != null)
      {
        var product = apiResult.Data;
        dto = new ProductFormDto
        {
          Id = product.Id,
          Title = product.Title,
          Description = product.Description,
          Category = product.Category,
          Price = product.Price,
          Stock = product.Stock,
          Brand = product.Brand ?? string.Empty
        };
      }
      else if (apiResult.StatusCode == HttpStatusCode.NotFound)
      {
        dto = new ProductFormDto
        {
          Id = id,
          Title = $"Product #{id}",
          Description = "Deskripsi product",
          Category = "",
          Price = 0,
          Stock = 0,
          Brand = ""
        };
      }
      else
      {
        TempData["ErrorMessage"] = GetErrorMessage(apiResult.StatusCode);
        return RedirectToAction("Index");
      }

      await PopulateCategoriesAndBrandsAsync();

      return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormDto dto)
    {
      if (!IsAdmin)
      {
        TempData["ErrorMessage"] = "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.";
        return RedirectToAction("Index");
      }

      if (!ModelState.IsValid)
      {
        await PopulateCategoriesAndBrandsAsync();
        return View(dto);
      }

      var product = new Product
      {
        Id = id,
        Title = dto.Title,
        Description = dto.Description,
        Category = dto.Category,
        Price = dto.Price,
        Stock = dto.Stock,
        Brand = dto.Brand
      };

      var apiResult = await _productService.UpdateProductAsync(id, product);

      if (!apiResult.IsSuccess && apiResult.StatusCode != HttpStatusCode.NotFound)
      {
        ViewBag.ErrorMessage = GetErrorMessage(apiResult.StatusCode);
        await PopulateCategoriesAndBrandsAsync();
        return View(dto);
      }

      TempData["SuccessMessage"] = $"Product '{product.Title}' berhasil diubah.";
      return RedirectToAction("Index");
    }

    private async Task PopulateCategoriesAndBrandsAsync()
    {
      var categoriesResult = await _productService.GetCategoriesAsync();
      ViewBag.Categories = categoriesResult.Data?.Select(c => c.Slug).ToList() ?? new List<string>();
      ViewBag.Brands = await _productService.GetBrandsAsync();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      if (!IsAdmin)
      {
        TempData["ErrorMessage"] = "Anda tidak memiliki hak akses untuk melakukan aktivitas ini.";
        return RedirectToAction("Index");
      }

      var apiResult = await _productService.DeleteProductAsync(id);

      if (!apiResult.IsSuccess)
      {
        TempData["ErrorMessage"] = GetErrorMessage(apiResult.StatusCode);
        return RedirectToAction("Index");
      }

      TempData["SuccessMessage"] = "Product berhasil dihapus.";
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Download(string format, string? search = null)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      var products = await _productService.GetProductsForExportAsync(search);

      if (format == "excel")
      {
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Products");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Category";
        worksheet.Cell(1, 4).Value = "Price";
        worksheet.Cell(1, 5).Value = "Stock";
        worksheet.Cell(1, 6).Value = "Rating";
        worksheet.Cell(1, 7).Value = "Brand";

        for (int i = 0; i < products.Count; i++)
        {
          var p = products[i];
          int row = i + 2;

          worksheet.Cell(row, 1).Value = p.Id;
          worksheet.Cell(row, 2).Value = p.Title;
          worksheet.Cell(row, 3).Value = p.Category;
          worksheet.Cell(row, 4).Value = p.Price;
          worksheet.Cell(row, 5).Value = p.Stock;
          worksheet.Cell(row, 6).Value = p.Rating;
          worksheet.Cell(row, 7).Value = p.Brand;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "products.xlsx");
      }
      else
      {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Id,Title,Category,Price,Stock,Rating,Brand");

        foreach (var p in products)
        {
          csv.AppendLine(
              $"{p.Id},\"{p.Title}\",\"{p.Category}\",{p.Price},{p.Stock},{p.Rating},\"{p.Brand}\"");
        }

        var bytes = System.Text.Encoding.UTF8.GetPreamble()
            .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        return File(bytes, "text/csv", "products.csv");
      }
    }
  }
}
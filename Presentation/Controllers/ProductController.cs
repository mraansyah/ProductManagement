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

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      const int pageSize = 10;

      if (page < 1) page = 1;

      var result = await _productService.GetProductsAsync(page, pageSize, search);

      if (result == null)
      {
        ViewBag.ErrorMessage = "Data product gagal dimuat. Silakan coba kembali.";
        return View(new ProductListViewModel());
      }

      var viewModel = new ProductListViewModel
      {
        Products = result.Products,
        CurrentPage = page,
        PageSize = pageSize,
        TotalItems = result.Total,
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

      var product = await _productService.GetProductByIdAsync(id);

      if (product == null)
      {
        TempData["ErrorMessage"] = "Product tidak ditemukan.";
        return RedirectToAction("Index");
      }

      return View(product);
    }

    // GET: /Product/Create
    public async Task<IActionResult> Create()
    {
      var categories = await _productService.GetCategoriesAsync();
      ViewBag.Categories = categories.Select(c => c.Slug).ToList();

      return View(new ProductFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormDto dto)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _productService.GetCategoriesAsync();
        ViewBag.Categories = categories.Select(c => c.Slug).ToList();
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

      var result = await _productService.CreateProductAsync(product);

      if (result == null)
      {
        ViewBag.ErrorMessage = "Gagal menambahkan product. Silakan coba kembali.";
        var categories2 = await _productService.GetCategoriesAsync();
        ViewBag.Categories = categories2.Select(c => c.Slug).ToList();
        return View(dto);
      }

      TempData["SuccessMessage"] = $"Product '{result.Title}' berhasil ditambahkan.";
      TempData["NewProduct"] = System.Text.Json.JsonSerializer.Serialize(result);
      return RedirectToAction("Index");
    }

    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var token = await GetValidAccessTokenAsync();
      if (token == null)
      {
        return RedirectToAction("Login", "Account");
      }

      var product = await _productService.GetProductByIdAsync(id);

      if (product == null)
      {
        TempData["ErrorMessage"] = "Product tidak ditemukan.";
        return RedirectToAction("Index");
      }

      var dto = new ProductFormDto
      {
        Id = product.Id,
        Title = product.Title,
        Description = product.Description,
        Category = product.Category,
        Price = product.Price,
        Stock = product.Stock,
        Brand = product.Brand ?? string.Empty
      };

      var categories = await _productService.GetCategoriesAsync();
      ViewBag.Categories = categories.Select(c => c.Slug).ToList();

      return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormDto dto)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _productService.GetCategoriesAsync();
        ViewBag.Categories = categories.Select(c => c.Slug).ToList();
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

      var result = await _productService.UpdateProductAsync(id, product);

      if (result == null)
      {
        ViewBag.ErrorMessage = "Gagal mengubah product. Silakan coba kembali.";
        var categories2 = await _productService.GetCategoriesAsync();
        ViewBag.Categories = categories2.Select(c => c.Slug).ToList();
        return View(dto);
      }

      TempData["SuccessMessage"] = $"Product '{result.Title}' berhasil diubah.";
      return RedirectToAction("Index");
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

      var success = await _productService.DeleteProductAsync(id);

      if (!success)
      {
        TempData["ErrorMessage"] = "Gagal menghapus product. Silakan coba kembali.";
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
using Business.DTOs;
using Data.ApiClients;
using Data.Models;

namespace Business.Services
{
  public class ProductService : IProductService
  {
    private readonly IProductApiClient _productApiClient;

    private const double RatingThreshold = 4.0;
    private const int LowStockThreshold = 10;

    public ProductService(IProductApiClient productApiClient)
    {
      _productApiClient = productApiClient;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
      var result = await _productApiClient.GetAllAsync(limit: 0, skip: 0);

      var summary = new DashboardSummaryDto();

      if (result == null || result.Products.Count == 0)
      {
        return summary;
      }

      var products = result.Products;

      summary.TotalProduct = result.Total;

      summary.TotalCategory = products
          .Select(p => p.Category)
          .Distinct()
          .Count();

      summary.ProductRatingAboveFour = products
          .Count(p => p.Rating > RatingThreshold);

      summary.ProductLowStock = products
          .Count(p => p.Stock < LowStockThreshold);

      summary.ProductByCategory = products
          .GroupBy(p => p.Category)
          .ToDictionary(g => g.Key, g => g.Count());

      summary.StockGroup = new Dictionary<string, int>
      {
        ["Habis"] = products.Count(p => p.Stock == 0),
        ["Rendah"] = products.Count(p => p.Stock > 0 && p.Stock < LowStockThreshold),
        ["Aman"] = products.Count(p => p.Stock >= LowStockThreshold)
      };

      summary.LatestProducts = products
          .OrderByDescending(p => p.Id)
          .Take(10)
          .Select(p => new LatestProductDto
          {
            Id = p.Id,
            Title = p.Title,
            Category = p.Category,
            Price = p.Price,
            Stock = p.Stock,
            Rating = p.Rating
          })
          .ToList();

      return summary;
    }

    public async Task<ProductListResponse?> GetProductsAsync(int page, int pageSize, string? search)
    {
      int skip = (page - 1) * pageSize;

      if (!string.IsNullOrWhiteSpace(search))
      {
        return await _productApiClient.SearchAsync(search, pageSize, skip);
      }

      return await _productApiClient.GetAllAsync(pageSize, skip);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
      return await _productApiClient.GetByIdAsync(id);
    }

    public async Task<Product?> CreateProductAsync(Product product)
    {
      return await _productApiClient.CreateAsync(product);
    }

    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
      return await _productApiClient.UpdateAsync(id, product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
      return await _productApiClient.DeleteAsync(id);
    }

    public async Task<List<ProductCategory>> GetCategoriesAsync()
    {
      var categories = await _productApiClient.GetCategoriesAsync();
      return categories ?? new List<ProductCategory>();
    }

    public async Task<List<Product>> GetProductsForExportAsync(string? search)
    {
      ProductListResponse? result;

      if (!string.IsNullOrWhiteSpace(search))
      {
        result = await _productApiClient.SearchAsync(search, limit: 0, skip: 0);
      }
      else
      {
        result = await _productApiClient.GetAllAsync(limit: 0, skip: 0);
      }

      return result?.Products ?? new List<Product>();
    }
  }
}
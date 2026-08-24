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
      var apiResult = await _productApiClient.GetAllAsync(limit: 0, skip: 0);

      var summary = new DashboardSummaryDto();

      if (!apiResult.IsSuccess || apiResult.Data == null || apiResult.Data.Products.Count == 0)
      {
        return summary;
      }

      var products = apiResult.Data.Products;

      summary.TotalProduct = apiResult.Data.Total;

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
            Brand = p.Brand,
            Price = p.Price,
            Stock = p.Stock,
            Rating = p.Rating
          })
          .ToList();

      return summary;
    }

    public async Task<ApiResult<ProductListResponse>> GetProductsAsync(int page, int pageSize, string? search)
    {
      int skip = (page - 1) * pageSize;

      if (!string.IsNullOrWhiteSpace(search))
      {
        return await _productApiClient.SearchAsync(search, pageSize, skip);
      }

      return await _productApiClient.GetAllAsync(pageSize, skip);
    }

    public async Task<ApiResult<Product>> GetProductByIdAsync(int id)
    {
      return await _productApiClient.GetByIdAsync(id);
    }

    public async Task<ApiResult<Product>> CreateProductAsync(Product product)
    {
      return await _productApiClient.CreateAsync(product);
    }

    public async Task<ApiResult<Product>> UpdateProductAsync(int id, Product product)
    {
      return await _productApiClient.UpdateAsync(id, product);
    }

    public async Task<ApiResult<bool>> DeleteProductAsync(int id)
    {
      return await _productApiClient.DeleteAsync(id);
    }

    public async Task<ApiResult<List<ProductCategory>>> GetCategoriesAsync()
    {
      return await _productApiClient.GetCategoriesAsync();
    }

    public async Task<List<string>> GetBrandsAsync()
    {
      var apiResult = await _productApiClient.GetAllAsync(limit: 0, skip: 0);
      if (!apiResult.IsSuccess || apiResult.Data == null)
      {
        return new List<string>();
      }

      return apiResult.Data.Products
          .Where(p => !string.IsNullOrWhiteSpace(p.Brand))
          .Select(p => p.Brand!)
          .Distinct()
          .OrderBy(b => b)
          .ToList();
    }

    public async Task<List<Product>> GetProductsForExportAsync(string? search)
    {
      ApiResult<ProductListResponse> apiResult;

      if (!string.IsNullOrWhiteSpace(search))
      {
        apiResult = await _productApiClient.SearchAsync(search, limit: 0, skip: 0);
      }
      else
      {
        apiResult = await _productApiClient.GetAllAsync(limit: 0, skip: 0);
      }

      return apiResult.Data?.Products ?? new List<Product>();
    }
  }
}
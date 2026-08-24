using Business.DTOs;
using Data.Models;

namespace Business.Services
{
  public interface IProductService
  {
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<ApiResult<ProductListResponse>> GetProductsAsync(int page, int pageSize, string? search);
    Task<ApiResult<Product>> GetProductByIdAsync(int id);
    Task<ApiResult<Product>> CreateProductAsync(Product product);
    Task<ApiResult<Product>> UpdateProductAsync(int id, Product product);
    Task<ApiResult<bool>> DeleteProductAsync(int id);
    Task<ApiResult<List<ProductCategory>>> GetCategoriesAsync();
    Task<List<string>> GetBrandsAsync();
    Task<List<Product>> GetProductsForExportAsync(string? search);
  }
}
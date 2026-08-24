using Business.DTOs;
using Data.Models;

namespace Business.Services
{
  public interface IProductService
  {
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<ProductListResponse?> GetProductsAsync(int page, int pageSize, string? search);
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product?> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(int id, Product product);
    Task<bool> DeleteProductAsync(int id);
  }
}
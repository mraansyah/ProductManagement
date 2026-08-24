using Data.Models;

namespace Data.ApiClients
{
  public interface IProductApiClient
  {
    Task<ApiResult<ProductListResponse>> GetAllAsync(int limit = 0, int skip = 0);
    Task<ApiResult<ProductListResponse>> SearchAsync(string query, int limit = 0, int skip = 0);
    Task<ApiResult<Product>> GetByIdAsync(int id);
    Task<ApiResult<Product>> CreateAsync(Product product);
    Task<ApiResult<Product>> UpdateAsync(int id, Product product);
    Task<ApiResult<bool>> DeleteAsync(int id);
    Task<ApiResult<List<ProductCategory>>> GetCategoriesAsync();
  }
}
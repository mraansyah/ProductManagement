using Data.Models;

namespace Data.ApiClients
{
  public interface IProductApiClient
  {
    Task<ProductListResponse?> GetAllAsync(int limit = 0, int skip = 0);
    Task<ProductListResponse?> SearchAsync(string query, int limit = 0, int skip = 0);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
  }
}
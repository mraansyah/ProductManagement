using System.Net.Http.Json;
using Data.Models;

namespace Data.ApiClients
{
  public class ProductApiClient : IProductApiClient
  {
    private readonly HttpClient _httpClient;

    public ProductApiClient(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task<ProductListResponse?> GetAllAsync(int limit = 0, int skip = 0)
    {
      var response = await _httpClient.GetAsync($"products?limit={limit}&skip={skip}");

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<ProductListResponse>();
    }

    public async Task<ProductListResponse?> SearchAsync(string query, int limit = 0, int skip = 0)
    {
      var response = await _httpClient.GetAsync($"products/search?q={Uri.EscapeDataString(query)}&limit={limit}&skip={skip}");

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<ProductListResponse>();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
      var response = await _httpClient.GetAsync($"products/{id}");

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<Product?> CreateAsync(Product product)
    {
      var response = await _httpClient.PostAsJsonAsync("products/add", product);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
      var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var response = await _httpClient.DeleteAsync($"products/{id}");
      return response.IsSuccessStatusCode;
    }
  }
}
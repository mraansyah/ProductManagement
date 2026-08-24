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

    public async Task<ApiResult<ProductListResponse>> GetAllAsync(int limit = 0, int skip = 0)
    {
      var response = await _httpClient.GetAsync($"products?limit={limit}&skip={skip}");

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<ProductListResponse>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<ProductListResponse>();
      return ApiResult<ProductListResponse>.Success(data!, response.StatusCode);
    }

    public async Task<ApiResult<ProductListResponse>> SearchAsync(string query, int limit = 0, int skip = 0)
    {
      var response = await _httpClient.GetAsync($"products/search?q={Uri.EscapeDataString(query)}&limit={limit}&skip={skip}");

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<ProductListResponse>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<ProductListResponse>();
      return ApiResult<ProductListResponse>.Success(data!, response.StatusCode);
    }

    public async Task<ApiResult<Product>> GetByIdAsync(int id)
    {
      var response = await _httpClient.GetAsync($"products/{id}");

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<Product>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<Product>();
      return ApiResult<Product>.Success(data!, response.StatusCode);
    }

    public async Task<ApiResult<Product>> CreateAsync(Product product)
    {
      var response = await _httpClient.PostAsJsonAsync("products/add", product);

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<Product>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<Product>();
      return ApiResult<Product>.Success(data!, response.StatusCode);
    }

    public async Task<ApiResult<Product>> UpdateAsync(int id, Product product)
    {
      var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<Product>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<Product>();
      return ApiResult<Product>.Success(data!, response.StatusCode);
    }

    public async Task<ApiResult<bool>> DeleteAsync(int id)
    {
      var response = await _httpClient.DeleteAsync($"products/{id}");

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<bool>.Failure(response.StatusCode);
      }

      return ApiResult<bool>.Success(true, response.StatusCode);
    }

    public async Task<ApiResult<List<ProductCategory>>> GetCategoriesAsync()
    {
      var response = await _httpClient.GetAsync("products/categories");

      if (!response.IsSuccessStatusCode)
      {
        return ApiResult<List<ProductCategory>>.Failure(response.StatusCode);
      }

      var data = await response.Content.ReadFromJsonAsync<List<ProductCategory>>();
      return ApiResult<List<ProductCategory>>.Success(data!, response.StatusCode);
    }
  }
}
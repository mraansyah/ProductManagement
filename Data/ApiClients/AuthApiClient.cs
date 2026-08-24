using System.Net.Http.Headers;
using System.Net.Http.Json;
using Data.Models;

namespace Data.ApiClients
{
  public class AuthApiClient : IAuthApiClient
  {
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task<LoginResponse?> LoginAsync(LoginApiRequest request)
    {
      var response = await _httpClient.PostAsJsonAsync("auth/login", request);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<AuthUser?> GetMeAsync(string accessToken)
    {
      var requestMessage = new HttpRequestMessage(HttpMethod.Get, "auth/me");
      requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var response = await _httpClient.SendAsync(requestMessage);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<AuthUser>();
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
      var body = new { refreshToken = refreshToken, expiresInMins = 30 };
      var response = await _httpClient.PostAsJsonAsync("auth/refresh", body);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
  }
}
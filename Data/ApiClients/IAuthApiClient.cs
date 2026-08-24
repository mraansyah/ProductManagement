using Data.Models;

namespace Data.ApiClients
{
  public interface IAuthApiClient
  {
    Task<LoginResponse?> LoginAsync(LoginApiRequest request);
    Task<AuthUser?> GetMeAsync(string accessToken);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
  }
}
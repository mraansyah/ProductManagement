using Business.DTOs;
using Data.ApiClients;
using Data.Models;

namespace Business.Services
{
  public class AuthService : IAuthService
  {
    private readonly IAuthApiClient _authApiClient;

    public AuthService(IAuthApiClient authApiClient)
    {
      _authApiClient = authApiClient;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequestDto dto)
    {
      var apiRequest = new LoginApiRequest
      {
        Username = dto.Username,
        Password = dto.Password,
        Expire = 30
      };

      return await _authApiClient.LoginAsync(apiRequest);
    }

    public async Task<AuthUser?> GetCurrentUserAsync(string accessToken)
    {
      return await _authApiClient.GetMeAsync(accessToken);
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
      return await _authApiClient.RefreshTokenAsync(refreshToken);
    }
  }
}
using Business.DTOs;
using Data.Models;

namespace Business.Services
{
  public interface IAuthService
  {
    Task<LoginResponse?> LoginAsync(LoginRequestDto dto);
    Task<AuthUser?> GetCurrentUserAsync(string accessToken);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
  }
}
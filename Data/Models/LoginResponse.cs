using System.Text.Json.Serialization;

namespace Data.Models
{
  public class LoginResponse : AuthUser
  {
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
  }
}
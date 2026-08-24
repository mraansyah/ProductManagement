using System.Text.Json.Serialization;

namespace Data.Models
{
  public class LoginApiRequest
  {
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("expiresInMins")]
    public int Expire { get; set; } = 30;
  }
}
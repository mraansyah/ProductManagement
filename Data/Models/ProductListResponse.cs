using System.Text.Json.Serialization;

namespace Data.Models
{
  public class ProductListResponse
  {
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("skip")]
    public int Skip { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
  }
}
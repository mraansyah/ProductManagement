namespace Business.DTOs
{
  public class DashboardSummaryDto
  {
    public int TotalProduct { get; set; }
    public int TotalCategory { get; set; }
    public int ProductRatingAboveFour { get; set; }
    public int ProductLowStock { get; set; }

    public Dictionary<string, int> ProductByCategory { get; set; } = new();

    public Dictionary<string, int> StockGroup { get; set; } = new();

    public List<LatestProductDto> LatestProducts { get; set; } = new();
  }

  public class LatestProductDto
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public double Rating { get; set; }
  }
}
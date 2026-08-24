using Data.Models;

namespace Data.ViewModels
{
  public class ProductListViewModel
  {
    public List<Product> Products { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public string? SearchQuery { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
  }
}
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
  public class ProductFormDto
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Title tidak boleh kosong")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description wajib diisi")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category wajib diisi")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price wajib diisi")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price harus lebih besar dari 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock wajib diisi")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock tidak boleh negatif")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Brand wajib diisi")]
    public string Brand { get; set; } = string.Empty;
  }
}
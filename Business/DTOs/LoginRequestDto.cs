using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
  public class LoginRequestDto
  {
    [Required(ErrorMessage = "Username wajib diisi")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    public string Password { get; set; } = string.Empty;
  }
}
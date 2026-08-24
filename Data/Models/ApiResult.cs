using System.Net;

namespace Data.Models
{
  public class ApiResult<T>
  {
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }

    public static ApiResult<T> Success(T data, HttpStatusCode statusCode)
    {
      return new ApiResult<T>
      {
        IsSuccess = true,
        StatusCode = statusCode,
        Data = data
      };
    }

    public static ApiResult<T> Failure(HttpStatusCode statusCode)
    {
      return new ApiResult<T>
      {
        IsSuccess = false,
        StatusCode = statusCode,
        Data = default
      };
    }
  }
}
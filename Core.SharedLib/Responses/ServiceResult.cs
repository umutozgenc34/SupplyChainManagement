using System.Net;
using System.Text.Json.Serialization;

namespace Core.Application.Responses;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonIgnore]
    public HttpStatusCode Status { get; set; }

    public static ServiceResult SuccessResult(string message = "Success") =>
        new() { Success = true, Message = message, Status = HttpStatusCode.OK };

    public static ServiceResult Failure(string message, HttpStatusCode status = HttpStatusCode.BadRequest) =>
        new() { Success = false, Message = message, Status = status };

    public static ServiceResult Failure(string message, List<string> errors, HttpStatusCode status = HttpStatusCode.BadRequest) =>
        new() { Success = false, Message = message, Errors = errors, Status = status };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> SuccessResult(T data, string message = "Success", HttpStatusCode status = HttpStatusCode.OK) =>
        new() { Success = true, Data = data, Message = message, Status = status };

    public new static ServiceResult<T> Failure(string message, HttpStatusCode status = HttpStatusCode.BadRequest) =>
        new() { Success = false, Message = message, Status = status };
}

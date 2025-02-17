namespace Core.Application.Responses;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResult SuccessResult(string message = "Success") =>
        new() { Success = true, Message = message };

    public static ServiceResult Failure(string message) =>
        new() { Success = false, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> SuccessResult(T data, string message = "Success") =>
        new() { Success = true, Data = data, Message = message };

    public new static ServiceResult<T> Failure(string message) =>
        new() { Success = false, Message = message };
}
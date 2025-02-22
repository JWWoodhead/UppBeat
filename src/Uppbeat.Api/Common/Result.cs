namespace Uppbeat.Api.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? value, string? error, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, null, 200);
    public static Result<T> NotFound(string error = "Resource not found") => new Result<T>(false, default, error, 404);
    public static Result<T> BadRequest(string error) => new Result<T>(false, default, error, 400);
}

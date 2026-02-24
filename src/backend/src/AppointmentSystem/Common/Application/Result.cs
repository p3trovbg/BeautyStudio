namespace AppointmentSystem.Common.Application;

/// <summary>
/// A discriminated result type for service operations. Encapsulates either a
/// successful value or a list of error messages, avoiding exceptions for expected failures.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public class Result<T>
{
    /// <summary>The success value, if any.</summary>
    public T? Value { get; }
    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }
    /// <summary>Whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>Error messages if the operation failed.</summary>
    public IReadOnlyList<string> Errors { get; }
    /// <summary>Machine-readable error code.</summary>
    public string? ErrorCode { get; }

    private Result(T value) { IsSuccess = true; Value = value; Errors = Array.Empty<string>(); }
    private Result(IReadOnlyList<string> errors, string? errorCode = null) { IsSuccess = false; Errors = errors; ErrorCode = errorCode; }

    /// <summary>Creates a successful result.</summary>
    public static Result<T> Success(T value) => new(value);
    /// <summary>Creates a failed result.</summary>
    public static Result<T> Failure(string error, string? errorCode = null) => new(new[] { error }, errorCode);
    /// <summary>Creates a failed result with multiple errors.</summary>
    public static Result<T> Failure(IReadOnlyList<string> errors, string? errorCode = null) => new(errors, errorCode);
}

/// <summary>Non-generic result for operations that return no value on success.</summary>
public class Result
{
    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }
    /// <summary>Whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>Error messages if the operation failed.</summary>
    public IReadOnlyList<string> Errors { get; }
    /// <summary>Machine-readable error code.</summary>
    public string? ErrorCode { get; }

    private Result(bool isSuccess, IReadOnlyList<string>? errors = null, string? errorCode = null)
    { IsSuccess = isSuccess; Errors = errors ?? Array.Empty<string>(); ErrorCode = errorCode; }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true);
    /// <summary>Creates a failed result.</summary>
    public static Result Failure(string error, string? errorCode = null) => new(false, new[] { error }, errorCode);
    /// <summary>Creates a failed result with multiple errors.</summary>
    public static Result Failure(IReadOnlyList<string> errors, string? errorCode = null) => new(false, errors, errorCode);
}

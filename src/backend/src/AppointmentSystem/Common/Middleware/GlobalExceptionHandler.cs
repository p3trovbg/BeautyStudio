using AppointmentSystem.Common.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Common.Middleware;

/// <summary>
/// Global exception handler converting unhandled exceptions to RFC 7807 ProblemDetails.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="GlobalExceptionHandler"/>.</summary>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    /// <summary>Handles an exception by writing a ProblemDetails response.</summary>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, problemDetails) = exception switch
        {
            DomainException domainEx => (StatusCodes.Status400BadRequest, new ProblemDetails
            {
                Title = domainEx.ErrorCode,
                Detail = domainEx.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7807"
            }),
            _ => (StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7807"
            })
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}

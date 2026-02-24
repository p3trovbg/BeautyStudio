namespace AppointmentSystem.Common.Domain;

/// <summary>
/// Base domain exception for all domain-level errors.
/// </summary>
public class DomainException : Exception
{
    /// <summary>A machine-readable error code.</summary>
    public string ErrorCode { get; }

    /// <summary>Initializes a new instance of <see cref="DomainException"/>.</summary>
    public DomainException(string message, string errorCode = "DOMAIN_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

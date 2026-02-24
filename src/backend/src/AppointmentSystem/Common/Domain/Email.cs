using System.Text.RegularExpressions;

namespace AppointmentSystem.Common.Domain;

/// <summary>
/// Value object representing a validated email address.
/// </summary>
public sealed partial class Email : IEquatable<Email>
{
    /// <summary>The email address value.</summary>
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>Creates a validated <see cref="Email"/>.</summary>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email address cannot be empty.", "INVALID_EMAIL");

        email = email.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(email))
            throw new DomainException($"'{email}' is not a valid email address.", "INVALID_EMAIL");

        return new Email(email);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    /// <inheritdoc />
    public bool Equals(Email? other) => other is not null && Value == other.Value;
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();
    /// <inheritdoc />
    public override string ToString() => Value;
    /// <summary>Implicit conversion from Email to string.</summary>
    public static implicit operator string(Email email) => email.Value;
}

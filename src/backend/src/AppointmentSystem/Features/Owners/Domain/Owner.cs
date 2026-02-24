using AppointmentSystem.Common.Domain;

namespace AppointmentSystem.Features.Owners.Domain;

/// <summary>
/// Represents a business owner providing services.
/// </summary>
public class Owner : BaseEntity
{
    private Owner(string fullName, Email email, string? phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    /// <summary>The owner's full name.</summary>
    public string FullName { get; private set; }
    /// <summary>The owner's validated email address.</summary>
    public Email Email { get; private set; }
    /// <summary>Optional contact phone number.</summary>
    public string? PhoneNumber { get; private set; }

    // Required by EF Core
    private Owner() { FullName = null!; Email = null!; }

    /// <summary>Creates a new <see cref="Owner"/> instance.</summary>
    public static Owner Create(string fullName, Email email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.", "INVALID_OWNER_NAME");

        return new Owner(fullName, email, phoneNumber);
    }

    /// <summary>Updates the owner's details.</summary>
    public void Update(string fullName, Email email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.", "INVALID_OWNER_NAME");

        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}

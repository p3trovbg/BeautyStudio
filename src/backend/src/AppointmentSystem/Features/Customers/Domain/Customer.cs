using AppointmentSystem.Common.Domain;

namespace AppointmentSystem.Features.Customers.Domain;

/// <summary>
/// Represents a customer booking appointments.
/// </summary>
public class Customer : BaseEntity
{
    private Customer(string fullName, Email email, string? phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    /// <summary>The customer's full name.</summary>
    public string FullName { get; private set; }
    /// <summary>The customer's validated email address.</summary>
    public Email Email { get; private set; }
    /// <summary>Optional contact phone number.</summary>
    public string? PhoneNumber { get; private set; }

    // Required by EF Core
    private Customer() { FullName = null!; Email = null!; }

    /// <summary>Creates a new <see cref="Customer"/> instance.</summary>
    public static Customer Create(string fullName, Email email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.", "INVALID_CUSTOMER_NAME");

        return new Customer(fullName, email, phoneNumber);
    }

    /// <summary>Updates the customer's details.</summary>
    public void Update(string fullName, Email email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.", "INVALID_CUSTOMER_NAME");

        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}

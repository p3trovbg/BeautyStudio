namespace AppointmentSystem.Features.Customers.DTOs;

/// <summary>Details of a customer.</summary>
public record CustomerDto(Guid Id, string FullName, string Email, string? PhoneNumber, DateTime CreatedAt);

/// <summary>Payload to create a customer.</summary>
public record CreateCustomerDto(string FullName, string Email, string? PhoneNumber);

/// <summary>Payload to update a customer.</summary>
public record UpdateCustomerDto(string FullName, string Email, string? PhoneNumber);

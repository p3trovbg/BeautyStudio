namespace AppointmentSystem.Features.Owners.DTOs;

/// <summary>Details of an owner.</summary>
public record OwnerDto(Guid Id, string FullName, string Email, string? PhoneNumber, DateTime CreatedAt);

/// <summary>Payload to create an owner.</summary>
public record CreateOwnerDto(string FullName, string Email, string? PhoneNumber);

/// <summary>Payload to update an owner.</summary>
public record UpdateOwnerDto(string FullName, string Email, string? PhoneNumber);

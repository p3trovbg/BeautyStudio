using AppointmentSystem.Features.Appointments.Domain;

namespace AppointmentSystem.Features.Appointments.DTOs;

public record AppointmentDto(
    Guid Id,
    Guid OwnerId,
    string OwnerName,
    string OwnerEmail,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status,
    string? Notes,
    DateTime CreatedAt);

public record CreateAppointmentDto(
    Guid OwnerId,
    Guid CustomerId,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    string? Notes);

public record UpdateAppointmentDto(
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status,
    string? Notes);

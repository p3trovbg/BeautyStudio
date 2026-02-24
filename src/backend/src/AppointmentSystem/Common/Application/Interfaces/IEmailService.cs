namespace AppointmentSystem.Common.Application.Interfaces;

/// <summary>
/// Abstraction for sending transactional emails related to appointments.
/// </summary>
public interface IEmailService
{
    /// <summary>Sends a booking confirmation email.</summary>
    Task SendBookingConfirmationAsync(Features.Appointments.DTOs.AppointmentDto appointment, CancellationToken ct);
    /// <summary>Sends a cancellation notice.</summary>
    Task SendCancellationNoticeAsync(Features.Appointments.DTOs.AppointmentDto appointment, CancellationToken ct);
    /// <summary>Sends a reminder email 24 hours before the appointment.</summary>
    Task SendReminderAsync(Features.Appointments.DTOs.AppointmentDto appointment, CancellationToken ct);
}

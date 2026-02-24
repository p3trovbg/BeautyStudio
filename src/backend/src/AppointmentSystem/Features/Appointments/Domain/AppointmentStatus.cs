namespace AppointmentSystem.Features.Appointments.Domain;

/// <summary>Status of an appointment.</summary>
public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

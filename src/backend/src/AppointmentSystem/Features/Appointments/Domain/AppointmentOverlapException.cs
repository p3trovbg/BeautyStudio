using AppointmentSystem.Common.Domain;

namespace AppointmentSystem.Features.Appointments.Domain;

/// <summary>
/// Exception thrown when an appointment overlaps with an existing one.
/// </summary>
public class AppointmentOverlapException : DomainException
{
    /// <summary>Initializes a new instance of <see cref="AppointmentOverlapException"/>.</summary>
    public AppointmentOverlapException(Guid ownerId, DateTime start, DateTime end)
        : base($"The time slot from {start:g} to {end:g} is not available for owner {ownerId}.", "APPOINTMENT_OVERLAP")
    {
    }
}
